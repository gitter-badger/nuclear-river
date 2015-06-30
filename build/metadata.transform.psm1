Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\metadata.servicebus.psm1" -DisableNameChecking

function Get-XdtTransformMetadata($EnvType, $Country, $Index){
	$xdt = @(
		'Common\log4net.Release.config'
		'Common\Erm.Release.config'
		"Templates\Erm.Test.$Country.config"
	)

	return $xdt
}

function Get-RegexTransformMetadata($EnvName, $Index){

	$regex = @{}
	$regex += @{ '{EnvNum}' = "$Index" }
	$regex += Get-SharedAccessKeyMetadata $EnvName

	return $regex
}

function Get-TransformMetadata ($EnvName, $EnvType, $Country, $Index){
	
	$metadata = @{
		'Xdt' = Get-XdtTransformMetadata $EnvType $Country $Index
		'Regex' = Get-RegexTransformMetadata $EnvName $Index
	}

	return $metadata
}

Export-ModuleMember -Function Get-TransformMetadata