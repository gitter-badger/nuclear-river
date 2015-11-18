Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\unittests.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking

$RunnerPackageInfo = Get-PackageInfo '2Gis.NuClear.DataTest.Runner'
$RunnerPath = Join-Path $RunnerPackageInfo.VersionedDir "tools\2Gis.NuClear.DataTest.Runner.exe"

function Run-DataTests ($Projects, $entryPointMetadataKey, $bulkToolExecutable){

	if ($Projects -eq $null){
		return
	}

	$testLocations = @{}

	foreach($project in $Projects){

		$projectName = [System.IO.Path]::GetFileNameWithoutExtension($Project.Name)
		$outDir = Join-Path $Metadata.Common.Dir.TempPersist "$entryPointMetadataKey\$projectName"

		$buildFileName = Create-BuildFile $project.FullName -Properties @{
			'OutDir' = $outDir
		}
		Add-MSBuildQueue $buildFileName $outDir $entryPointMetadataKey

		$testLocations[$projectName] = $outDir
	}

	Invoke-MSBuildQueue -PublishArtifacts $false

	$assemblies = @()
	foreach($testLocation in $testLocations.GetEnumerator()){
		$testAssemblies = Get-ChildItem $testLocation.Value -Filter "*$($testLocation.Key)*.dll"
		foreach($testAssembly in $testAssemblies){
			$assemblies += $testAssembly.FullName
		}
	}
	
    $isTeamCity = Test-Path 'Env:\TEAMCITY_VERSION'
    if($isTeamCity) {
        & $RunnerPath $assemblies --teamcity=true --bulktool=$bulkToolExecutable
    }
    else {
        & $RunnerPath $assemblies --bulktool=$bulkToolExecutable
    }

		if ($lastExitCode -ne 0) {
		throw "Command failed with exit code $lastExitCode"
	}
}