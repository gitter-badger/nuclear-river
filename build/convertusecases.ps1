Set-StrictMode -Version Latest
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
	$destProcessPath = Join-Path $destDirName '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe'

	$sourceDirPath = Join-Path $packageInfo.VersionedDir 'tools'
	$configFileName = '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe.config'
	$configFilePath = Join-Path $sourceDirPath $configFileName
	$transformedConfig = Get-TransformedConfig $configFilePath
	$tempConfigFilePath = Join-Path $commonMetadata.Dir.Temp $configFileName
	$transformedConfig.Save($tempConfigFilePath)

	$entryPointMetadata = Get-Metadata 'Replication.EntryPoint'
	foreach($targetHost in $entryPointMetadata.TargetHosts){

		$session = Get-CachedSession $targetHost
		Invoke-Command $session {
			$processPath = "${Env:ProgramFiles}\$using:destProcessPath"
		
			$process = Get-Process | where { $_.MainModule.FileName -eq $processPath }
			if ($process -ne $null){
				Write-Host "Killing process by pid $($process.Id)"
				Stop-Process -Id $process.Id -Force
			}
		}

		Invoke-MSDeploy `
		-Source "dirPath=""$sourceDirPath""" `
		-Dest "dirPath=""%ProgramFiles%\$destDirName""" `
		-HostName $targetHost

		Invoke-MSDeploy `
		-Source "filePath=""$tempConfigFilePath""" `
		-Dest "filePath=""%ProgramFiles%\$destDirName\$configFileName""" `
		-HostName $targetHost

		Invoke-Command $session {
			$processPath = "${Env:ProgramFiles}\$using:destProcessPath"
			Start-Process -FilePath $processPath -ArgumentList @('infiniteloop')

			$jobName = $using:destDirName

			$job = Get-ScheduledJob $jobName
			if ($job -eq $null){
				$scriptBlock = { Start-Process -FilePath $processPath -ArgumentList @('infiniteloop') }
				$trigger = New-JobTrigger -AtStartup
				$option = New-ScheduledJobOption -RunElevated
				Register-ScheduledJob -Name "Tool1" -ScriptBlock $scriptBlock -Trigger $trigger -ScheduledJobOption $option
			}

			$job.Run()
		}
	}
}

