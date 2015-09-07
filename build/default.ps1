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

Task Build-OData -Precondition { $Metadata['Web.OData'] -ne $null } {
	$projectFileName = Get-ProjectFileName '.' 'Web.OData'
	Build-WebPackage $projectFileName 'Web.OData'
}
Task Deploy-OData -Depends Take-ODataOffline -Precondition { $Metadata['Web.OData'] -ne $null } {
	Deploy-WebPackage 'Web.OData'
	Validate-WebSite 'Web.OData' 'CustomerIntelligence/$metadata'
}

Task Take-ODataOffline -Precondition { $Metadata['Web.OData'] -ne $null } {
	Take-WebsiteOffline 'Web.OData'
}

Task Build-TaskService -Precondition { $Metadata['Replication.EntryPoint'] -ne $null } {
	$projectFileName = Get-ProjectFileName '.' 'Replication.EntryPoint'
	Build-WinService $projectFileName 'Replication.EntryPoint'
}

Task Deploy-TaskService -Depends Import-WinServiceModule, Take-TaskServiceOffline -Precondition { $Metadata['Replication.EntryPoint'] -ne $null } {
	Deploy-WinService 'Replication.EntryPoint'
}

Task Take-TaskServiceOffline -Depends Import-WinServiceModule -Precondition { $Metadata['Replication.EntryPoint'] -ne $null } {
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

Task Update-Schemas -Depends Build-ReplicationLibs -Precondition { $Metadata['UpdateSchemas'] -ne $null -and $Metadata['UpdateSchemas'].Count -ne 0 } {

	$libDir = Get-Artifacts 'ReplicationLibs'
	$scriptFilePath = Join-Path $PSScriptRoot 'replicate.ps1'
	$config = Get-ReplicationConfig

	$sqlScriptsDir = Join-Path $Metadata.Common.Dir.Solution 'Schemas'

	& $scriptFilePath `
	-LibDir $libDir `
	-Config $config `
	-SqlScriptsDir $sqlScriptsDir `
	-UpdateSchemas $Metadata['UpdateSchemas']
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
Build-TaskService, `
Build-ConvertUseCasesService

Task Deploy-Packages -depends `
Take-ODataOffline, `
Take-TaskServiceOffline, `
Update-Schemas, `
Deploy-ConvertUseCasesService, `
Deploy-OData, `
Deploy-TaskService