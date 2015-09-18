Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\servicebus.psm1" -DisableNameChecking

Task Build-ConvertUseCasesService -Precondition { $Metadata['ConvertUseCasesService'] } {

	$tempDir = Get-ConvertUseCasesServiceTempDir

	$configFileName = Join-Path $tempDir '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe.config'
	$transformedConfig = Get-TransformedConfig $configFileName 'ConvertUseCasesService'
	$transformedConfig.Save($configFileName)

	if ($Metadata['ServiceBus'] -and $Metadata.ServiceBus.UseCaseRoute -eq 'Production'){
		Apply-ProductionConnectionString $configFileName
	}

	Publish-Artifacts $tempDir 'ConvertUseCasesService'
}

function Apply-ProductionConnectionString ($configFileName) {

	$connectionString = Get-ProductionConnectionString

	$transformXml = @"
<configuration xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
	<connectionStrings>
	    <add name="Source" connectionString="$connectionString"
			   xdt:Transform="SetAttributes" xdt:Locator="Match(name)"/>
	</connectionStrings>
</configuration>
"@
	$transformXmlFileName = Join-Path $Metadata.Common.Dir.Temp 'ConvertUseCases.Source.transform.config'
	Set-Content $transformXmlFileName $transformXml -Encoding UTF8

	$transformedConfig = Apply-XdtTransform $configFileName $transformXmlFileName
	$transformedConfig.Save($configFileName)
}

function Get-ProductionConnectionString {
	$configFileName = Join-Path $Metadata.Common.Dir.Solution 'Environments\ConvertUseCases.Production.Russia.config'
	[xml]$config = Get-Content $configFileName -Raw

	$connectionString = Get-ConnectionString $config 'Source'
	return $connectionString
}

function Get-ConvertUseCasesServiceTempDir  {
	$packageInfo = Get-PackageInfo '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases'
	$toolsDir = $packageInfo.VersionedDir

	$tempDir = Join-Path $Metadata.Common.Dir.Temp 'ConvertUseCasesService'
	Copy-Item $toolsDir $tempDir -Force -Recurse
	
	return $tempDir
}

Task Create-Topics -Precondition { $Metadata['ServiceBus'] } {

	$artifacts = Get-Artifacts 'ConvertUseCasesService'
	$configFileName = Join-Path $artifacts '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe.config'
	[xml]$config = Get-Content $configFileName -Raw

	$serviceBusMetadata = $Metadata.ServiceBus
	$routeMetadata = $serviceBusMetadata.Routes[$serviceBusMetadata.UseCaseRoute]

	if ($serviceBusMetadata.UseCaseRoute -ne 'Production'){
		$productionConnectionString = Get-ProductionConnectionString
		$productionRouteMetadata = $serviceBusMetadata.Routes.Production
		Delete-Subscription $productionConnectionString $productionRouteMetadata.SourceTopic $productionRouteMetadata.SourceSubscription
	}

	$sourceConnectionString = Get-ConnectionString $config 'Source'
	Create-Topic $sourceConnectionString $routeMetadata.SourceTopic
	Create-Subscription $sourceConnectionString $routeMetadata.SourceTopic $routeMetadata.SourceSubscription

	$destConnectionString = Get-ConnectionString $config 'Dest'
	Delete-Topic $destConnectionString 'topic.advancedsearch' # временно, потом удалить
	Delete-Topic $destConnectionString 'topic.performedoperations(.*)import'

	Create-Topic $destConnectionString $routeMetadata.DestTopic
	Create-Subscription $destConnectionString $routeMetadata.DestTopic $routeMetadata.DestSubscription
}

Task Deploy-ConvertUseCasesService -Depends Build-ConvertUseCasesService, Create-Topics -Precondition { $Metadata['ConvertUseCasesService'] } {
	
	Load-WinServiceModule 'ConvertUseCasesService'
	Take-WinServiceOffline 'ConvertUseCasesService'
	Deploy-WinService 'ConvertUseCasesService'
}