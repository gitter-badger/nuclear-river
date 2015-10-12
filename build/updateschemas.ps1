Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\sql.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking

Task Update-Schemas -Precondition { $Metadata['UpdateSchemas'] } {

	$projectFileName = Get-ProjectFileName 'Replication' 'Replication.Bulk'
	$projectDir = Split-Path $projectFileName
	$configFileName = Join-Path $projectDir 'app.config'
	[xml]$config = Get-TransformedConfig $configFileName 'Replication.Bulk'

	$connectionString = Get-ConnectionString $config 'CustomerIntelligence'
	Create-Database $connectionString

	$sqlDir = Join-Path $Metadata.Common.Dir.Solution 'CustomerIntelligence\Schemas'
	Update-Schemas $sqlDir $connectionString
}

function Create-Database ($connectionString) {

	$builder = New-Object System.Data.Common.DbConnectionStringBuilder
	$builder.set_ConnectionString($connectionString)
	$initialCatalog = $builder['Initial Catalog']
	$builder['Initial Catalog'] = $null

	$connection = Create-SqlConnection $builder.ConnectionString

	$sql = Get-Content (Join-Path $Metadata.Common.Dir.Solution 'Replication\Schemas\Database.sql') -Raw
	$sql = $sql -replace '\$\(Database\)', $initialCatalog

	Write-Host "Database.sql..."
	Execute-Sql $sql $connection
}

function Update-Schemas ($sqlDir, $connectionString) {

	$connection = Create-SqlConnection $connectionString

	foreach ($schema in $Metadata['UpdateSchemas']) {
		$sql = Get-Content (Join-Path $sqlDir "$schema.sql") -Raw

		Write-Host "$schema.sql..."
		Execute-Sql $sql $connection
	}
}