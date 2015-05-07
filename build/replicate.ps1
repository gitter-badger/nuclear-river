param($LibDir, $ConnectionStrings, $SqlScriptsDir)

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

function Replicate-ErmToFacts (){
	$schema = [NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]

	$ermConnection = $sqlServerTools::CreateDataConnection($ConnectionStrings.Erm).AddMappingSchema($schema::Erm)
	$ermContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.ErmContext($ermConnection)
	$factsTransformationContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.FactsTransformationContext($ermContext)

	$factsConnection = $sqlServerTools::CreateDataConnection($ConnectionStrings.CustomerIntelligence).AddMappingSchema($schema::Facts)
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
}

function Create-Database {
	$builder = New-Object System.Data.Common.DbConnectionStringBuilder
	$builder.set_ConnectionString($ConnectionStrings.CustomerIntelligence)

	$initialCatalog = $builder['Initial Catalog']
	$builder['Initial Catalog'] = $null

	$connectionString = $builder.ConnectionString
	$connection = $sqlServerTools::CreateDataConnection($connectionString)

	$command = @"
if (not exists(select * from sys.databases where name = '$initialCatalog'))
	exec ('create database $initialCatalog')
"@

	Exec-Command $connection $command
}

function Create-Tables {
	$connection = $sqlServerTools::CreateDataConnection($ConnectionStrings.CustomerIntelligence)
	$sqlScripts = Get-ChildItem $SqlScriptsDir -Filter '*.sql'
	foreach($sqlScript in $sqlScripts){
		$command = Get-Content $sqlScript.FullName -Raw
		Exec-Command $connection $command
	}
}

function Exec-Command ($connection, [string]$command){
	$commands = $command.Split([string[]]@("`r`nGO", "`r`ngo"), 'RemoveEmptyEntries')

	foreach($command in $commands){
		$commandInfo = New-Object LinqToDB.Data.CommandInfo($connection, $command)
		[void]$commandInfo.Execute()
	}
}

Create-Database
Create-Tables
#Replicate-ErmToFacts

"Done"