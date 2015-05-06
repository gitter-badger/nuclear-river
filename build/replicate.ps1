param($LibDir, $ConnectionStrings)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

function Load-Assemblies {
	$files =  Get-ChildItem $LibDir -Filter '*.dll'
	foreach($file in $files) {
		$bytes = [System.IO.File]::ReadAllBytes($file.FullName)
		[void][System.Reflection.Assembly]::Load($bytes)
	}
}

Load-Assemblies

# aliases
$sqlServerTools = [LinqToDB.DataProvider.SqlServer.SqlServerTools]
$schema = [NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]

$ermConnection = $sqlServerTools::CreateDataConnection($ConnectionStrings.Erm).AddMappingSchema($schema::Erm)
$ermContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.ErmContext($ermConnection)
$factsTransformationContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.FactsTransformationContext($ermContext)

$factsConnection = $sqlServerTools::CreateDataConnection($ConnectionStrings.Facts).AddMappingSchema($schema::Facts)
$GetAttributeMethod = $factsConnection.MappingSchema.GetType().GetMethod('GetAttribute', [Type[]]@([Type], [bool])).MakeGenericMethod([LinqToDB.Mapping.TableAttribute])

$dtos = $factsTransformationContext.GetType().GetProperties() |
Where { [System.Linq.IQueryable].IsAssignableFrom($_.PropertyType) } |
ForEach {

	$pocoType = $_.PropertyType.GetGenericArguments()[0]
	$tableAttribute = $GetAttributeMethod.Invoke($factsConnection.MappingSchema, [object[]]@($pocoType, $false))

	$tableName = $tableAttribute.Name
	if ($tableName -eq $null){
		$tableName = $pocoType.Name
	}

	$query = $_.GetValue($factsTransformationContext)

	return @{
		'SchemaName' = $tableAttribute.Schema
		'TableName' = $tableName
		'Query' = $query
	}
}

$bulkCopyOptions = New-Object LinqToDB.Data.BulkCopyOptions
$bulkCopyOptions.BulkCopyTimeout = 0

foreach($dto in $dtos){

	$fullTableName = "[$($dto.SchemaName)].[$($dto.TableName)]"
	Write-Host "$fullTableName..."

	$commandInfo = New-Object LinqToDB.Data.CommandInfo($factsConnection, "truncate table $fullTableName")
	[void]$commandInfo.Execute()

	[void][LinqToDB.Data.DataConnectionExtensions]::BulkCopy($factsConnection, $bulkCopyOptions, $dto.Query)
}

"Done"