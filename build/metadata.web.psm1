Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

$DomainNames = @{
	'Chile' = 'cl'
	'Cyprus' = 'com.cy'
	'Czech' = 'cz'
	'Emirates' = 'ae'
	'Russia' = 'ru'
	'Ukraine' = 'ua'
	'Kazakhstan' = 'kz'
}

function Get-TargetHostsMetadata ($EnvType, $Country, $EntryPoint) {

	switch ($EnvType) {
		'Test' {
			switch ($Country) {
				'Russia' {
					return @{ 'TargetHosts' = @('uk-erm-test01') }
				}
				default {
					return @{ 'TargetHosts' = @('uk-erm-test02') }
				}
			}
		}
		'Edu' {
			return @{ 'TargetHosts' = @('uk-erm-edu01') }
		}
		'Business' {
			return @{ 'TargetHosts' = @('uk-erm-edu02') }
		}
		'Production' {
			return @{ 'TargetHosts' = @('uk-erm-iis03') }
		}
		'Int' {
			switch ($Country) {
				'Russia' {
					return @{ 'TargetHosts' = @('uk-test-int02') }
				}
				default {
					return @{ 'TargetHosts' = @('uk-test-int01') }
				}
			}
		}
		'Load' {
			return @{ 'TargetHosts' = @('uk-erm-iis12') }
		}
		default {
			throw "Unknown EntryPoint '$EntryPoint'"
		}
	}
}

function Get-ValidateWebsiteMetadata ($EnvType) {
	switch($EnvType){
		{ @('Production', 'Load') -contains $_ } {
			return @{ 'ValidateWebsite' = $false }
		}
		default {
			return @{ 'ValidateWebsite' = $true }
		}
	}
}

function Get-IisAppPathMetadata ($EnvType, $Country, $EntryPoint, $Index) {

	switch ($EntryPoint) {
		'2Gis.Erm.UI.Web.Mvc' { $prefix = "web-app$Index" }
		'2Gis.Erm.API.WCF.Operations' { $prefix = "basic-operations$Index.api" }
		'2Gis.Erm.API.WCF.MoDi' { $prefix = "money-distribution$Index.api" }
		'2Gis.Erm.API.WCF.Metadata' { $prefix = "metadata$Index.api" }
		'2Gis.Erm.API.WCF.OrderValidation' { $prefix = "order-validation$Index.api" }
		'2Gis.Erm.API.WCF.Operations.Special' { $prefix = "financial-operations$Index.api" }
		'2Gis.Erm.API.WCF.Releasing' { $prefix = "releasing$Index.api" }
		'2Gis.Erm.UI.Desktop.WPF' { $prefix = "wpf-app$Index" }
		# line added
		'Web.OData' { $prefix = "search$Index.api" }
		default {
			return @{}
		}
	}

	$envTypeLower = $EnvType.ToLowerInvariant()
	$domain = $DomainNames[$Country]

	switch ($EnvType) {
		'Production' {
			return @{ 'IisAppPath' = "$prefix.prod.erm.2gis.$domain" }
		}
		default {
			return @{ 'IisAppPath' = "$prefix.$envTypeLower.erm.2gis.$domain" }
		}
	}
}

function Get-TakeOfflineMetadata ($EnvType) {
	switch($EnvType){
		'Production' {
			return @{ 'TakeOffline' = $false }
		}
		default {
			return @{ 'TakeOffline' = $true }
		}
	}
}

function Get-OptionsMetadata ($Country) {

	switch ($Country){
		'Russia' {
			return @{
				'OptionModi' = $true
			}
		}
		default {
			return @{
				'OptionModi' = $false
			}
		}
	}
}

function Get-WebMetadata ($EnvType, $Country, $EntryPoint, $Index) {

	$metadata = @{}
	$metadata += Get-ValidateWebsiteMetadata $EnvType
	$metadata += Get-TargetHostsMetadata $EnvType $Country $EntryPoint
	$metadata += Get-IisAppPathMetadata $EnvType $Country $EntryPoint $Index
	$metadata += Get-TakeOfflineMetadata $EnvType
	$metadata += Get-OptionsMetadata $Country
	
	$metadata += @{
		'EntrypointType' = 'Web'
	}
	
	return $metadata
}

Export-ModuleMember -Function Get-WebMetadata