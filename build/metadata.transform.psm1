Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\metadata.servicebus.psm1" -DisableNameChecking

function Get-XdtTransformMetadata($Context){
	$xdt = @(
		'Common\log4net.Release.config'
		'Common\Erm.Release.config'
	)

	switch($Context.EnvType){
		'Test' {
			$xdt += @(
				"Templates\Erm.Test.$($Context.Country).config"
				"Templates\ConvertUseCases.Test.$($Context.Country).config"
				)
		}
		'Production' {
			$xdt += @(
				"Erm.Production.$($Context.Country).config"
				"ConvertUseCases.Production.$($Context.Country).config"
				)
		}
	}

	return $xdt
}

function Get-RegexTransformMetadata($Context){

	$regex = @{}

	if ($Context['Index']){
		$regex += @{ '{EnvNum}' = $Context['Index'] }
	}

	$regex += Get-SharedAccessKeyMetadata $Context

	return $regex
}

function Get-TransformMetadata ($Context) {
	
	return @{
		'Transform' = @{
			'Xdt' = Get-XdtTransformMetadata $Context
			'Regex' = Get-RegexTransformMetadata $Context
		}
	}
}

Export-ModuleMember -Function Get-TransformMetadata