Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

if (Test-Path 'Env:\TEAMCITY_VERSION') {
	FormatTaskName "##teamcity[progressMessage '{0}']"
}

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\versioning.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\unittests.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\web.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\winservice.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\winrm.psm1" -DisableNameChecking

Task Default -depends Hello
Task Hello { "Билдскрипт запущен без цели, укажите цель" }

Task Set-BuildNumber {
	$commonMetadata = Get-Metadata 'Common'
	
	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		Write-Host "##teamcity[buildNumber '$($commonMetadata.Version.SemanticVersion)']"
	}
}

Task Update-AssemblyInfo {
	$commonMetadata = Get-Metadata 'Common'

	$assemblyInfos = Get-ChildItem $commonMetadata.Dir.Solution -Filter 'AssemblyInfo.Version.cs' -Recurse
	Update-AssemblyInfo $assemblyInfos
}

Task Run-UnitTests -depends Set-BuildNumber, Update-AssemblyInfo{
	$SolutionRelatedAllProjectsDir = '.'
	
	$projects = Find-Projects $SolutionRelatedAllProjectsDir '*Tests*'
	foreach($project in $Projects){
		$buildFileName = Create-BuildFile $project.FullName
		Invoke-MSBuild $buildFileName
	}

	Run-UnitTests $projects
}

Task Build-OData -depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' 'Web.OData'
	$entryPointMetadata = Get-Metadata 'Web.OData'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}

Task Deploy-OData {
	$projectFileName = Get-ProjectFileName '.' 'Web.OData'
	$entryPointMetadata = Get-Metadata 'Web.OData'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'CustomerIntelligence/$metadata'
}

Task Build-Packages -depends Set-BuildNumber, Build-OData
Task Deploy-Packages -depends Deploy-OData