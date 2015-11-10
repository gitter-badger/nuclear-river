Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\sql.psm1" -DisableNameChecking

Task Update-Schemas -Precondition { $Metadata['UpdateSchemas'] } {

	$projectFileName = Get-ProjectFileName '.' 'Replication.EntryPoint'
	$projectDir = Split-Path $projectFileName
	$configFileName = Join-Path $projectDir 'app.config'
	[xml]$config = Get-TransformedConfig $configFileName 'Replication.EntryPoint'

	$sqlDir = Join-Path $Metadata.Common.Dir.Solution 'Schemas'
	Update-Schemas $config $sqlDir
}

function Update-Schemas ($config, $sqlDir) {
	$updateSchemasMetadata = $Metadata['UpdateSchemas']

	foreach ($schema in $updateSchemasMetadata.Schemas) {

		$connectionString = Get-ConnectionString $config $updateSchemasMetadata.ConnectionString[$schema]
		$connection = Create-SqlConnection $connectionString

		$sql = Get-Content (Join-Path $sqlDir "$schema.sql") -Raw

		Write-Host "$schema.sql..."
		Execute-Sql $sql $connection
	}
}