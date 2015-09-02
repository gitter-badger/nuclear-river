Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\metadata.web.psm1" -DisableNameChecking

function Get-QuartzConfigMetadata ($EnvType, $Country, $Index){

	switch ($EnvType){
		'Test' {
			switch ($Country){
				'Russia' {
					$quartzConfigs = @('Templates\quartz.Test.Russia.config')
					
					$alterQuartzConfigs = @()
				}
				'Emirates' {
					$quartzConfigs = @('Templates\quartz.Test.Emirates.config')
					$alterQuartzConfigs = @()
				}
				default {
					$quartzConfigs = @('Templates\quartz.Test.MultiCulture.config')
					$alterQuartzConfigs = @()
				}
			}
		}
		'Production' {
			switch ($Country){
				'Russia' {
					$quartzConfigs = @('quartz.Production.Russia.config')
					$alterQuartzConfigs = @()
				}
				'Emirates' {
					$quartzConfigs = @('quartz.Production.Emirates.config')
					$alterQuartzConfigs = @()
				}
				default {
					$quartzConfigs = @('quartz.Production.MultiCulture.config')
					$alterQuartzConfigs = @()
				}
			}
		}
		default {
			switch ($Country){
				'Russia' {
					$quartzConfigs = @("quartz.$EnvType.Russia.config")
					$alterQuartzConfigs = @('Templates\quartz.Test.Russia.config')
				}
				'Emirates' {
					$quartzConfigs = @("quartz.$EnvType.Emirates.config")
					$alterQuartzConfigs = @('Templates\quartz.Test.Emirates.config')
				}
				default {
					$quartzConfigs = @("quartz.$EnvType.MultiCulture.config")
					$alterQuartzConfigs = @('Templates\quartz.Test.MultiCulture.config')
				}
			}
		}
	}

	return @{
		'QuartzConfigs' =  $quartzConfigs
		'AlterQuartzConfigs' = $alterQuartzConfigs
	}
}

function Get-TargetHostsMetadata ($EnvType, $Country, $Index){

	$webMetadata = Get-WebMetadata $EnvType $Country '2Gis.Erm.TaskService.Installer' $Index

	switch ($EnvType) {
		'Production' {
			return @{ 'TargetHosts' = @('uk-erm-sb01', 'uk-erm-sb03', 'uk-erm-sb04') }
		}
		'Load' {
			return @{ 'TargetHosts' = @('uk-erm-iis10', 'uk-erm-iis11', 'uk-erm-iis12') }
		}
		default {
			return @{'TargetHosts' = $webMetadata.TargetHosts}
		}
	}
}

function Get-ServiceNameMetadata ($EntryPoint, $Index) {
	switch ($EntryPoint) {
		'Replication.EntryPoint' {
			return @{
				'ServiceName' = 'AdvSearch'
				'ServiceDisplayName' = '2GIS ERM AdvancedSearch Replication Service'
			}
		}
		'ConvertUseCasesService' {
			return @{
				'ServiceName' = 'ConvertUseCases'
				'ServiceDisplayName' = '2GIS ERM AdvancedSearch Convert UseCases Service'
			}
		}
	}
}

function Get-TaskServiceMetadata ($EnvType, $Country, $EntryPoint, $Index) {

	$metadata = @{}
	$metadata += Get-TargetHostsMetadata $EnvType $Country $Index
	$metadata += Get-QuartzConfigMetadata $EnvType $Country $Index
	$metadata += Get-ServiceNameMetadata $EntryPoint $Index
	
	$metadata += @{
		'EntrypointType' = 'Desktop'
	}
	
	return $metadata
}

Export-ModuleMember -Function Get-TaskServiceMetadata