Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\deploy.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\unittests.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking

Include "$BuildToolsRoot\psake\tasks.ps1"
Include 'convertusecases.ps1'

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

Task QueueBuild-OData -Precondition { $Metadata['Web.OData'] } {
	$projectFileName = Get-ProjectFileName 'Querying' 'Querying.Web.OData'
	QueueBuild-WebPackage $projectFileName 'Web.OData'
}
Task Deploy-OData -Depends Take-ODataOffline -Precondition { $Metadata['Web.OData'] } {
	Deploy-WebPackage 'Web.OData'
	Validate-WebSite 'Web.OData' 'CustomerIntelligence/$metadata'
}

Task Take-ODataOffline -Precondition { $Metadata['Web.OData'] } {
	Take-WebsiteOffline 'Web.OData'
}

Task QueueBuild-TaskService -Precondition { $Metadata['Replication.EntryPoint'] } {
	$projectFileName = Get-ProjectFileName 'Replication' 'Replication.EntryPoint'
	QueueBuild-AppPackage $projectFileName 'Replication.EntryPoint'
}

Task Deploy-TaskService -Depends Import-WinServiceModule, Take-TaskServiceOffline -Precondition { $Metadata['Replication.EntryPoint'] } {
	Deploy-WinService 'Replication.EntryPoint'
}

Task Take-TaskServiceOffline -Depends Import-WinServiceModule -Precondition { $Metadata['Replication.EntryPoint'] } {
	Take-WinServiceOffline 'Replication.EntryPoint'
}

Task Import-WinServiceModule {
	Load-WinServiceModule 'Replication.EntryPoint'
}

Task Build-ReplicationLibs {
	$projectFileName = Get-ProjectFileName 'Replication' 'Replication.Core'
	$buildFile = Create-BuildFile $projectFileName
	Invoke-MSBuild $buildFile
	$conventionalArtifactFileName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $conventionalArtifactFileName 'ReplicationLibs'

	$projectFileName = Get-ProjectFileName 'Replication' 'Replication.OperationsProcessing'
	$buildFile = Create-BuildFile $projectFileName
	Invoke-MSBuild $buildFile
	$conventionalArtifactFileName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $conventionalArtifactFileName 'ReplicationLibs'
}

Task Update-Schemas -Depends Build-ReplicationLibs -Precondition { $Metadata['UpdateSchemas'] } {

	$libDir = Get-Artifacts 'ReplicationLibs'
	$scriptFilePath = Join-Path $PSScriptRoot 'replicate.ps1'
	$config = Get-ReplicationConfig

	$sqlScriptsDir = Join-Path $Metadata.Common.Dir.Solution 'CustomerIntelligence\Schemas'

	& $scriptFilePath `
	-LibDir $libDir `
	-Config $config `
	-SqlScriptsDir $sqlScriptsDir `
	-UpdateSchemas $Metadata['UpdateSchemas']
}

function Get-ReplicationConfig {

	$projectFileName = Get-ProjectFileName 'Replication' 'Replication.EntryPoint'
	$projectDir = Split-Path $projectFileName
	$configFileName = Join-Path $projectDir 'app.config'
	[xml]$config = Get-TransformedConfig $configFileName 'Replication.EntryPoint'

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
QueueBuild-OData, `
QueueBuild-TaskService, `
Build-ConvertUseCasesService, `
Build-Queue

Task Deploy-Packages -depends `
Take-ODataOffline, `
Take-TaskServiceOffline, `
Update-Schemas, `
Deploy-ConvertUseCasesService, `
Deploy-OData, `
Deploy-TaskService