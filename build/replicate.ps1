param($LibDir, $Config, $SqlScriptsDir, [string[]]$UpdateSchemas)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\servicebus.psm1" -DisableNameChecking

# load libraries
Get-ChildItem $LibDir -Filter '*.dll' | ForEach { [void][System.Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($_.FullName)) }

function Replicate-Data {

	if($UpdateSchemas -contains 'ERM'){
		Replicate-ErmToFacts
		Replicate-FactsToCI
	}

	if($UpdateSchemas -contains 'CustomerIntelligence'){
		Replicate-FactsToCI
	}
}

function Replicate-ErmToFacts {

	$ermConnection = Create-SqlServerConnection $Config.ConnectionStrings.Erm
	[void]$ermConnection.AddMappingSchema([NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]::Erm)

	$ermContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.ErmContext($ermConnection)
	$factsTransformationContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.ErmFactsTransformationContext($ermContext)

	$factsConnection = Create-SqlServerConnection $Config.ConnectionStrings.CustomerIntelligence
	[void]$factsConnection.AddMappingSchema([NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]::Facts)

	$queryDtos = Get-QueryDtos $factsTransformationContext $factsConnection.MappingSchema
	Replicate-QueryDtoToConnection $factsConnection $queryDtos
}

function Replicate-FactsToCI {

	$factsConnection = Create-SqlServerConnection $Config.ConnectionStrings.CustomerIntelligence
	[void]$factsConnection.AddMappingSchema([NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]::Facts)

	$ermFactsContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.ErmFactsContext($factsConnection)
	$ciTransformationContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.CustomerIntelligenceTransformationContext($ermFactsContext)


	$ciConnection = Create-SqlServerConnection $Config.ConnectionStrings.CustomerIntelligence
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

		$commandInfo = New-Object LinqToDB.Data.CommandInfo($connection, "delete from $fullTableName")
		[void]$commandInfo.Execute()

		[void][LinqToDB.Data.DataConnectionExtensions]::BulkCopy($connection, $bulkCopyOptions, $queryDto.Query)
	}
}

function Create-Database {

	$builder = New-Object System.Data.Common.DbConnectionStringBuilder
	$builder.set_ConnectionString($Config.ConnectionStrings.CustomerIntelligence)
	$initialCatalog = $builder['Initial Catalog']
	$builder['Initial Catalog'] = $null

	$connection = Create-SqlServerConnection $builder.ConnectionString

	$command = @"
if (not exists(select * from sys.databases where name = '$initialCatalog'))
	exec ('create database $initialCatalog')
"@

	Exec-Command $connection $command
}

function Update-Schemas {

	$connection = Create-SqlServerConnection $Config.ConnectionStrings.CustomerIntelligence

	foreach ($schema in $UpdateSchemas) {
		$sqlScript = Get-Item (Join-Path $SqlScriptsDir "$schema.sql")
		Write-Host "$schema.sql..."

		$command = [System.IO.File]::ReadAllText($sqlScript.FullName)
		Exec-Command $connection $command
	}
}

function Create-SqlServerConnection ($ConnectionString){
	$connection = [LinqToDB.DataProvider.SqlServer.SqlServerTools]::CreateDataConnection($ConnectionString)
	$connection.CommandTimeout = 0

	return $connection
}

function Exec-Command ($connection, [string]$command){
	$commands = $command.Split([string[]]@("`nGO", "`ngo"), 'RemoveEmptyEntries')

	foreach($command in $commands){

		try{
			$commandInfo = New-Object LinqToDB.Data.CommandInfo($connection, $command)
			[void]$commandInfo.Execute()
		}
		catch [System.Data.SqlClient.SqlException]
		{
		   throw
		}
		catch {
			throw "Error executing command $command"
		}
	}
}

Create-Database
Update-Schemas
Replicate-Data

"Done"