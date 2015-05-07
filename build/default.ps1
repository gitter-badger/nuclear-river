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
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking

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

	$assemblyInfos = Get-ChildItem $commonMetadata.Dir.Solution -Filter 'AssemblyInfo*.cs' -Recurse
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

Task Build-ReplicationLibs {
	$projectFileName = Get-ProjectFileName '.' 'Replication'
	$buildFile = Create-BuildFile $projectFileName
	Invoke-MSBuild $buildFile
	$conventionalArtifactFileName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $conventionalArtifactFileName 'Replication'

	$projectFileName = Get-ProjectFileName '.' 'Replication.OperationsProcessing'
	$buildFile = Create-BuildFile $projectFileName
	Invoke-MSBuild $buildFile
	$conventionalArtifactFileName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $conventionalArtifactFileName 'ReplicationLibs'
}

Task Replicate-AllTables -Depends Build-ReplicationLibs {

	$libDir = Get-Artifacts 'ReplicationLibs'
	$scriptFilePath = Join-Path $PSScriptRoot 'replicate.ps1'
	$connectionStrings = Get-ConnectionStrings

	$sqlScriptsDir = Join-Path (Get-Metadata 'Common').Dir.Solution 'TestData'

	& $scriptFilePath `
	-LibDir $libDir `
	-ConnectionStrings $connectionStrings `
	-SqlScriptsDir $sqlScriptsDir
}

function Get-ConnectionStrings {

	$projectFileName = Get-ProjectFileName '.' 'Replication.EntryPoint'
	$projectDir = Split-Path $projectFileName
	$configFileName = Join-Path $projectDir 'app.config'
	[xml]$config = Get-TransformedConfig $configFileName

	return @{
		'Erm' = Get-ConnectionString $config 'Erm'
		'CustomerIntelligence' = Get-ConnectionString $config 'CustomerIntelligence'
		'ServiceBus' = Get-ConnectionString $config 'ServiceBus'
	}
}

Task Build-Packages -depends `
Set-BuildNumber, `
Build-OData, `
Build-TaskService

Task Deploy-Packages -depends `
Deploy-OData, `
Deploy-TaskService