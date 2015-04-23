param([string[]]$TaskList = @(), [hashtable]$Properties = @{})

if ($TaskList.Count -eq 0){
	$TaskList = @('Build-Packages')
}

if ($Properties.Count -eq 0){
 	$Properties.EnvironmentName = 'Test.21'
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------
cls

$Properties.SemanticVersion = '0.0.0'
$Properties.BuildFile = Join-Path $PSScriptRoot 'default.ps1'
$Properties.SolutionDir = Join-Path $PSScriptRoot '..'

$Properties.EnvironmentMetadata = @{
	'Test.21' = @{
		'Transform' = @{
			'Xdt' = @('Erm.Release.config')
			'Regex' = @{}
		}
		'Web.OData' = @{
			'ValidateWebsite' = $true
			'TargetHosts' = @('uk-erm-test01')
			'IisAppPath' = "search21.api.test.erm.2gis.ru"
		}
	}
}

# Restore-Packages
& {
	$NugetPath = Join-Path $Properties.SolutionDir '.nuget\NuGet.exe'
	if (!(Test-Path $NugetPath)){
		$webClient = New-Object System.Net.WebClient
		$webClient.UseDefaultCredentials = $true
		$webClient.Proxy.Credentials = $webClient.Credentials
		$webClient.DownloadFile('https://www.nuget.org/nuget.exe', $NugetPath)
	}
	$solution = Get-ChildItem $Properties.SolutionDir -Filter '*.sln'
	& $NugetPath @('restore', $solution.FullName, '-NonInteractive', '-Verbosity', 'quiet')
}

Import-Module "$($Properties.SolutionDir)\packages\2GIS.NuClear.BuildTools.0.0.35-TaskServ-f18ba0-59\tools\buildtools.psm1" -DisableNameChecking -Force
Run-Build $TaskList $Properties
