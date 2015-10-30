Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\metadata.servicebus.psm1" -DisableNameChecking

function Get-XdtMetadata($Context){
	$xdt = @()


	switch($Context.EntryPoint){
		'ConvertUseCasesService' {
			switch($Context.EnvType){
				'Test' {
					$xdt += @("Templates\ConvertUseCases.Test.$($Context.Country).config")
				}
				default {
					$xdt += @("ConvertUseCases.$($Context.EnvType).$($Context.Country).config")
				}
			}
		}
		default {
			$xdt += @(
				'Common\log4net.Release.config'
				'Common\Erm.Release.config'
			)

			switch($Context.EnvType){
				'Test' {
					$xdt += @("Templates\Erm.Test.$($Context.Country).config")
				}
				default {
					$xdt += @("Erm.$($Context.EnvType).$($Context.Country).config")
				}
			}
		}
	}

	return $xdt
}

function Get-RegexMetadata($Context){

	$regex = @{}

	if ($Context['Index']){
		$regex += @{ '{EnvNum}' = $Context['Index'] }
	}

	$serviceBusMetadata = Get-ServiceBusMetadata $Context
	if ($serviceBusMetadata.Count -ne 0){
		$routeMetadata = $serviceBusMetadata.ServiceBus.Routes[$serviceBusMetadata.ServiceBus.UseCaseRoute]
		$regex += @{
			'{SourceTopic}' = $routeMetadata.SourceTopic
			'{SourceSubscription}' = $routeMetadata.SourceSubscription
			'{DestTopic}' = $routeMetadata.DestTopic
		}
	}

	return $regex
}

function Get-TransformMetadata ($Context) {

	return @{
		'Transform' = @{
			'Xdt' = Get-XdtMetadata $Context
			'Regex' = Get-RegexMetadata $Context
		}
	}
}

Export-ModuleMember -Function Get-TransformMetadata