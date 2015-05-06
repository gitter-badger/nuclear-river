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
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking

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
	Build-WebPackage $projectFileName 'Web.OData'
}

Task Deploy-OData {
	Deploy-WebPackage 'Web.OData'
	Validate-WebSite 'Web.OData' 'CustomerIntelligence/$metadata'
}

Task Build-TaskService -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' 'Replication.EntryPoint'
	Build-WinService $projectFileName 'Replication.EntryPoint'
}

Task Deploy-TaskService -Depends Import-WinServiceModule, Take-TaskServiceOffline {
	Deploy-WinService 'Replication.EntryPoint'
}

Task Take-TaskServiceOffline -Depends Import-WinServiceModule {
	Take-WinServiceOffline 'Replication.EntryPoint'
}

Task Import-WinServiceModule {
	Load-WinServiceModule 'Replication.EntryPoint'
}

Task Build-Packages -depends `
Set-BuildNumber, `
Build-OData, `
Build-TaskService

Task Deploy-Packages -depends `
Deploy-OData, `
Deploy-TaskService