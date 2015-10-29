Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\unittests.psm1" -DisableNameChecking

Task QueueBuild-BulkTool -Precondition { $Metadata['Replication.Bulk'] } {

	$projectFileName = Get-ProjectFileName '.' 'Replication.Tests'
	QueueBuild-AppPackage $projectFileName 'Replication.Bulk'
}

Task Run-BulkTool -Precondition { $Metadata['Replication.Bulk'] } {

	$artifactName = Get-Artifacts 'Replication.Bulk'
	Apply-ReplicationConnectionStrings $artifactName

	$assemblies = Get-ChildItem $artifactName -Filter "*Tests*.dll" -Recurse

	foreach ($argument in $Metadata['Replication.Bulk'].Arguments){

		$lastExitCode = Invoke-NUnit $assemblies.FullName -Arguments @(
			'/include:' + $argument
			'/stoponerror'
		)

		if ($lastExitCode -ne 0){
			throw "Error then running bulk tool"
		}
	}
}

function Apply-ReplicationConnectionStrings ($artifactName){

	$configFileName = Join-Path $artifactName '2GIS.NuClear.AdvancedSearch.Replication.Tests.dll.config'

	$replicationConfig = Get-ReplicationEntryPointConfig
	$ermConnectionString = Get-ConnectionString $replicationConfig 'Erm'
	$ciConnectionString = Get-ConnectionString $replicationConfig 'CustomerIntelligence'

	$transformXml = @"
<configuration xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
	<connectionStrings>
		<add name="Erm" connectionString="" xdt:Transform="Replace" xdt:Locator="Match(name)"/>
		<add name="Facts" connectionString="" xdt:Transform="Replace" xdt:Locator="Match(name)"/>
		<add name="CustomerIntelligence" connectionString="" xdt:Transform="Replace" xdt:Locator="Match(name)"/>
	    <add name="ErmSqlServer" connectionString="$ermConnectionString"
			   xdt:Transform="SetAttributes" xdt:Locator="Match(name)"/>
	    <add name="FactsSqlServer" connectionString="$ciConnectionString"
			   xdt:Transform="SetAttributes" xdt:Locator="Match(name)"/>
	    <add name="CustomerIntelligenceSqlServer" connectionString="$ciConnectionString"
			   xdt:Transform="SetAttributes" xdt:Locator="Match(name)"/>
	</connectionStrings>
</configuration>
"@

	$transformXmlFileName = Join-Path $Metadata.Common.Dir.Temp 'Replication.Tests.transform.config'
	Set-Content $transformXmlFileName $transformXml -Encoding UTF8

	$transformedConfig = Apply-XdtTransform $configFileName $transformXmlFileName
	$transformedConfig.Save($configFileName)
}

function Get-ReplicationEntryPointConfig {
	$projectFileName = Get-ProjectFileName '.' 'Replication.EntryPoint'
	$projectDir = Split-Path $projectFileName
	$configFileName = Join-Path $projectDir 'app.config'
	[xml]$config = Get-TransformedConfig $configFileName 'Replication.EntryPoint'

	return $config
}