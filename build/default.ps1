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
	$entryPointMetadata = Get-Metadata 'Web.OData'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}

Task Deploy-OData {
	$projectFileName = Get-ProjectFileName '.' 'Web.OData'
	$entryPointMetadata = Get-Metadata 'Web.OData'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'CustomerIntelligence/$metadata'
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

Task Build-ReplicationLib {
	$projectFileName = Get-ProjectFileName '.' 'Replication'
	$buildFile = Create-BuildFile $projectFileName
	Invoke-MSBuild $buildFile

	$conventionalArtifactFileName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $conventionalArtifactFileName 'Replication'
}

Task Replicate-AllTables -Depends Build-ReplicationLib {

	$libDir = Get-Artifacts 'Replication'
	$scriptFilePath = Join-Path $PSScriptRoot 'replicate.ps1'
	$connectionStrings = Get-ConnectionStrings

	& $scriptFilePath `
	-LibDir $libDir `
	-ErmConString $connectionStrings.Erm `
	-FactsConString $connectionStrings.Facts
}

function Get-ConnectionStrings {
	#$projectFileName = Get-ProjectFileName '.' 'Replication.EntryPoint'
	return @{
		'Erm' = 'Data Source=uk-sql01;Initial Catalog=Erm21;Integrated Security=True'
		'Facts' = 'Data Source=uk-sql01;Initial Catalog=CustomerIntelligence21;Integrated Security=True'
		'CustomerIntelligence' = 'Data Source=uk-sql01;Initial Catalog=CustomerIntelligence21;Integrated Security=True'
	}
}

Task Build-Packages -depends `
Set-BuildNumber, `
Build-OData, `
Build-TaskService

Task Deploy-Packages -depends `
Deploy-OData, `
Deploy-TaskService