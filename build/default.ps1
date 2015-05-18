Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\unittests.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\web.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking

Task Default -depends Hello
Task Hello { "Билдскрипт запущен без цели, укажите цель" }

Task Run-UnitTests {
	$SolutionRelatedAllProjectsDir = '.'
	
	$projects = Find-Projects $SolutionRelatedAllProjectsDir '*Tests*'
	foreach($project in $Projects){
		$buildFileName = Create-BuildFile $project.FullName
		Invoke-MSBuild $buildFileName
	}

	Run-UnitTests $projects
}

Task Build-OData {
	$projectFileName = Get-ProjectFileName '.' 'Web.OData'
	Build-WebPackage $projectFileName 'Web.OData'
}

Task Deploy-OData -Depends Take-ODataOffline {
	Deploy-WebPackage 'Web.OData'
	Validate-WebSite 'Web.OData' 'CustomerIntelligence/$metadata'
}

Task Take-ODataOffline {
	Take-WebsiteOffline 'Web.OData'
}

Task Build-TaskService {
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
	Publish-Artifacts $conventionalArtifactFileName 'ReplicationLibs'

	$projectFileName = Get-ProjectFileName '.' 'Replication.OperationsProcessing'
	$buildFile = Create-BuildFile $projectFileName
	Invoke-MSBuild $buildFile
	$conventionalArtifactFileName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $conventionalArtifactFileName 'ReplicationLibs'
}

Properties { $OptionCreateEnvironment = $false }
Task Create-Environment -Depends Build-ReplicationLibs -Precondition { $OptionCreateEnvironment } {

	$libDir = Get-Artifacts 'ReplicationLibs'
	$scriptFilePath = Join-Path $PSScriptRoot 'replicate.ps1'
	$config = Get-ReplicationConfig

	$sqlScriptsDir = Join-Path (Get-Metadata 'Common').Dir.Solution 'TestData'

	& $scriptFilePath `
	-LibDir $libDir `
	-Config $config `
	-SqlScriptsDir $sqlScriptsDir
}

Properties { $OptionConvertUseCases = $false }
Task Convert-UseCases -Precondition { $OptionConvertUseCases } {
	$packageInfo = Get-PackageInfo '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases'

	$toolPath = Join-Path $packageInfo.VersionedDir 'tools\2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe'
	$config = Get-ReplicationConfig

	& $toolPath @($config.ConnectionStrings.ServiceBus)
	if ($lastExitCode -ne 0) {
		throw "Command failed with exit code $lastExitCode"
	}
}

function Get-ReplicationConfig {

	$projectFileName = Get-ProjectFileName '.' 'Replication.EntryPoint'
	$projectDir = Split-Path $projectFileName
	$configFileName = Join-Path $projectDir 'app.config'
	[xml]$config = Get-TransformedConfig $configFileName

	return @{
		'AppSettings' = @{
			'TransportEntityPath' = Get-AppSetting $config 'TransportEntityPath'		
		}
		'ConnectionStrings' = @{
			'Erm' = Get-ConnectionString $config 'Erm'
			'CustomerIntelligence' = Get-ConnectionString $config 'CustomerIntelligence'
			'ServiceBus' = Get-ConnectionString $config 'ServiceBus'
		}
	}
}

Task Build-Packages -depends `
Build-OData, `
Build-TaskService

Task Deploy-Packages -depends `
Take-ODataOffline, `
Take-TaskServiceOffline, `
Create-Environment, `
Convert-UseCases, `
Deploy-OData, `
Deploy-TaskService