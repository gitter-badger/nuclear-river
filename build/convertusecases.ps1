﻿Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msdeploy.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\winrm.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking

Properties { $OptionConvertUseCases = $false }
Task Deploy-ConvertUseCasesTool -Precondition { $OptionConvertUseCases } {
	$packageInfo = Get-PackageInfo '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases'
	
	$commonMetadata = Get-Metadata 'Common'
	$destDirName = "2GIS ERM AdvancedSearch ConvertUseCases Tool ($($commonMetadata.EnvironmentName))"

	$sourceDirPath = Join-Path $packageInfo.VersionedDir 'tools'
	$configFileName = '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe.config'
	$configFilePath = Join-Path $sourceDirPath $configFileName
	$transformedConfig = Get-TransformedConfig $configFilePath
	$tempConfigFilePath = Join-Path $commonMetadata.Dir.Temp $configFileName
	$transformedConfig.Save($tempConfigFilePath)

	$entryPointMetadata = Get-Metadata 'Replication.EntryPoint'
	foreach($targetHost in $entryPointMetadata.TargetHosts){

		Invoke-MSDeploy `
		-Source "dirPath=""$sourceDirPath""" `
		-Dest "dirPath=""%ProgramFiles%\$destDirName""" `
		-HostName $targetHost

		Invoke-MSDeploy `
		-Source "filePath=""$tempConfigFilePath""" `
		-Dest "filePath=""%ProgramFiles%\$destDirName\$configFileName""" `
		-HostName $targetHost

		$session = Get-CachedSession $targetHost
		Invoke-Command $session {
			$processPath = "${Env:ProgramFiles}\$using:destDirName\2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe"
		
			$process = Get-Process | where { $_.MainModule.ModuleName -eq $processPath }
			if ($process -ne $null){
				Stop-Process -Id $process.Id -Force
			}

			Start-Process -FilePath $processPath
		}
	}
}

