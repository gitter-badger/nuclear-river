Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\deploy.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\servicebus.psm1" -DisableNameChecking

Task Build-ConvertUseCasesService -Precondition { $Metadata['ConvertUseCasesService'] -and $Metadata['UseCaseRoute'] } {

	$tempDir = Get-ConvertUseCasesServiceTempDir

	$configFileName = Join-Path $tempDir '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe.config'
	$transformedConfig = Get-TransformedConfig $configFileName 'ConvertUseCasesService'
	$transformedConfig.Save($configFileName)

	if ($Metadata.UseCaseRoute.RouteName -match 'ERMProduction'){
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
	$toolsDir = Join-Path $packageInfo.VersionedDir 'tools'

	$tempDir = Join-Path $Metadata.Common.Dir.Temp 'ConvertUseCasesService'
	Copy-Item $toolsDir $tempDir -Force -Recurse
	
	return $tempDir
}

Task Create-Topics -Precondition { $Metadata['ConvertUseCasesService'] -and $Metadata['UseCaseRoute'] } {

	$artifacts = Get-Artifacts 'ConvertUseCasesService'
	$configFileName = Join-Path $artifacts '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe.config'
	[xml]$config = Get-Content $configFileName -Raw

	$useCaseRouteMetadata = $Metadata.UseCaseRoute

	if ($useCaseRouteMetadata.RouteName -notmatch 'ERMProduction'){
		$productionConnectionString = Get-ProductionConnectionString
		$productionRouteMetadata = $Metadata.ProductionUseCaseRoute
		Delete-Subscription $productionConnectionString $productionRouteMetadata.SourceTopic $productionRouteMetadata.SourceSubscription
	}

	$sourceConnectionString = Get-ConnectionString $config 'Source'
	Create-Topic $sourceConnectionString $useCaseRouteMetadata.SourceTopic -Properties @{
		'EnableBatchedOperations' = $true
		'SupportOrdering' = $true
	}
	Create-Subscription $sourceConnectionString $useCaseRouteMetadata.SourceTopic $useCaseRouteMetadata.SourceSubscription -Properties @{
		'EnableBatchedOperations' = $true
		'MaxDeliveryCount' = 0x7fffffff
	}

	$destConnectionString = Get-ConnectionString $config 'Dest'
	#Delete-Topic $destConnectionString 'topic.advancedsearch' # временно, потом удалить
	#Delete-Topic $destConnectionString 'topic.performedoperations(.*)import'

	Create-Topic $destConnectionString $useCaseRouteMetadata.DestTopic -Properties @{
		'EnableBatchedOperations' = $true
		'SupportOrdering' = $true
		'RequiresDuplicateDetection' = $true
	}
	Create-Subscription $destConnectionString $useCaseRouteMetadata.DestTopic $useCaseRouteMetadata.DestSubscription -Properties @{
		'EnableBatchedOperations' = $true
		'MaxDeliveryCount' = 0x7fffffff
	}
}

Task Deploy-ConvertUseCasesService -Depends Create-Topics -Precondition { $Metadata['ConvertUseCasesService'] -and $Metadata['UseCaseRoute'] } {
	
	Load-WinServiceModule 'ConvertUseCasesService'
	Take-WinServiceOffline 'ConvertUseCasesService'

	if ($Metadata.UseCaseRoute.RouteName -eq 'ERM'){
		Remove-WinService 'ConvertUseCasesService'
	} else {
		Deploy-WinService 'ConvertUseCasesService'
	}
}