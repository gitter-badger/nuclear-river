Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

if (Test-Path 'Env:\TEAMCITY_VERSION') {
	FormatTaskName "##teamcity[progressMessage '{0}']"
}

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\versioning.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\unittests.psm1" -DisableNameChecking

Task Default -depends Hello
Task Hello { "Билдскрипт запущен без цели, укажите цель" }

Task Set-BuildNumber {
	$version = Get-Version
	
	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		Write-Host "##teamcity[buildNumber '$($version.SemanticVersion)']"
	}
}

Task Update-AssemblyInfo {
	$globalDir = Join-Path $global:Context.Dir.Solution '.'
	$assemblyInfos = Get-ChildItem $globalDir -Filter 'AssemblyInfo.cs' -Recurse
	Update-AssemblyInfo $assemblyInfos
}

Task Build-NuGet -depends Set-BuildNumber, Update-AssemblyInfo {

	$SolutionRelatedAllProjectsDir = '.'

	$tempDir = Join-Path $global:Context.Dir.Temp 'NuGet'
	if (!(Test-Path $tempDir)){
		md $tempDir | Out-Null
	}

	Create-NuspecFiles $SolutionRelatedAllProjectsDir

	$projects = Find-Projects $SolutionRelatedAllProjectsDir -Exclude '*.Tests.csproj'
	
	# bug (https://nuget.codeplex.com/workitem/4013) NuGet fails to extract properties from PCL project
	# bug will be fixed in NuGet 3.0
	$Properties = @{
		'Version' = (Get-Version).SemanticVersion
		'Author' = '2GIS'
		'Description' = 'This is Release version of assembly'
	}
	
	Build-PackagesFromProjects $projects $tempDir $Properties
	
	Publish-Artifacts $tempDir 'NuGet'
}

Task Deploy-NuGet {
	$artifactName = Get-Artifacts 'NuGet'

	$source = 'http://nuget.2gis.local'
	$apiKey = ':enrbq rjl'

	$packges = Get-ChildItem $artifactName -Include '*.nupkg' -Exclude '*.symbols.nupkg' -Recurse
	Deploy-Packages $packges $source $apiKey
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