param($LibDir, $ConnectionStrings, $SqlScriptsDir)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

# load libraries
Get-ChildItem $LibDir -Filter '*.dll' | ForEach { [void][System.Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($_.FullName)) }

function Replicate-ErmToFacts {

	$ermConnection = Create-SqlServerConnection $ConnectionStrings.Erm
	[void]$ermConnection.AddMappingSchema([NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]::Erm)

	$ermContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.ErmContext($ermConnection)
	$factsTransformationContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.FactsTransformationContext($ermContext)

	$factsConnection = Create-SqlServerConnection $ConnectionStrings.CustomerIntelligence
	[void]$factsConnection.AddMappingSchema([NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]::Facts)

	$queryDtos = Get-QueryDtos $factsTransformationContext $factsConnection.MappingSchema
	Replicate-QueryDtoToConnection $factsConnection $queryDtos
}

function Replicate-FactsToCI {

	$factsConnection = Create-SqlServerConnection $ConnectionStrings.CustomerIntelligence
	[void]$factsConnection.AddMappingSchema([NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]::Facts)

	$factsContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.FactsContext($factsConnection)
	$ciTransformationContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.CustomerIntelligenceTransformationContext($factsContext)

	$ciConnection = Create-SqlServerConnection $ConnectionStrings.CustomerIntelligence
	[void]$ciConnection.AddMappingSchema([NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]::CustomerIntelligence)

	$queryDtos = Get-QueryDtos $ciTransformationContext $ciConnection.MappingSchema
	Replicate-QueryDtoToConnection $ciConnection $queryDtos
}

function Get-QueryDtos ($transformationContext, $mappingSchema) {

	$GetAttributeMethod = $mappingSchema.GetType().GetMethod('GetAttribute', [Type[]]@([Type], [bool])).MakeGenericMethod([LinqToDB.Mapping.TableAttribute])

	$queryDtos = $transformationContext.GetType().GetProperties() |
	Where { [System.Linq.IQueryable].IsAssignableFrom($_.PropertyType) } |
	ForEach {

		$pocoType = $_.PropertyType.GetGenericArguments()[0]
		$tableAttribute = $GetAttributeMethod.Invoke($mappingSchema, [object[]]@($pocoType, $false))

		$tableName = $tableAttribute.Name
		if ($tableName -eq $null){
			$tableName = $pocoType.Name
		}

		$query = $_.GetValue($transformationContext)

		return @{
			'SchemaName' = $tableAttribute.Schema
			'TableName' = $tableName
			'Query' = $query
		}
	}

	return $queryDtos
}

function Replicate-QueryDtoToConnection ($connection, $queryDtos){

	$bulkCopyOptions = New-Object LinqToDB.Data.BulkCopyOptions
	$bulkCopyOptions.BulkCopyTimeout = 0

	foreach($queryDto in $queryDtos){

		$fullTableName = "[$($queryDto.SchemaName)].[$($queryDto.TableName)]"
		Write-Host "$fullTableName..."

		$commandInfo = New-Object LinqToDB.Data.CommandInfo($connection, "truncate table $fullTableName")
		[void]$commandInfo.Execute()

		[void][LinqToDB.Data.DataConnectionExtensions]::BulkCopy($connection, $bulkCopyOptions, $queryDto.Query)
	}
}

function Create-Database {

	$builder = New-Object System.Data.Common.DbConnectionStringBuilder
	$builder.set_ConnectionString($ConnectionStrings.CustomerIntelligence)
	$initialCatalog = $builder['Initial Catalog']
	$builder['Initial Catalog'] = $null

	$connection = Create-SqlServerConnection $builder.ConnectionString

	$command = @"
if (not exists(select * from sys.databases where name = '$initialCatalog'))
	exec ('create database $initialCatalog')
"@

	Exec-Command $connection $command
}

function Create-Tables {

	Create-Database

	$connection = Create-SqlServerConnection $ConnectionStrings.CustomerIntelligence

	$sqlScripts = Get-ChildItem $SqlScriptsDir -Filter '*.sql'
	foreach($sqlScript in $sqlScripts){
		$command = Get-Content $sqlScript.FullName -Raw
		Exec-Command $connection $command
	}
}

function Create-SqlServerConnection ($ConnectionString){
	$connection = [LinqToDB.DataProvider.SqlServer.SqlServerTools]::CreateDataConnection($ConnectionString)
	$connection.CommandTimeout = 0

	return $connection
}

function Exec-Command ($connection, [string]$command){
	$commands = $command.Split([string[]]@("`r`nGO", "`r`ngo"), 'RemoveEmptyEntries')

	foreach($command in $commands){
		$commandInfo = New-Object LinqToDB.Data.CommandInfo($connection, $command)
		[void]$commandInfo.Execute()
	}
}

Create-Tables
Replicate-ErmToFacts
Replicate-FactsToCI

"Done"