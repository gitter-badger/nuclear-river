Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking

Task QueueBuild-BulkTool -Precondition { $Metadata['Replication.Bulk'] } {
	$projectFileName = Get-ProjectFileName 'Replication' 'Replication.Bulk'
	QueueBuild-AppPackage $projectFileName 'Replication.Bulk'
}

Task Run-BulkTool -Precondition { $Metadata['Replication.Bulk'] } {
	$artifactName = Get-Artifacts 'Replication.Bulk'

	$exePath = Join-Path $artifactName '2GIS.NuClear.AdvancedSearch.Replication.Bulk.exe'

	& $exePath $Metadata['Replication.Bulk'].Arguments | Write-Host

	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}