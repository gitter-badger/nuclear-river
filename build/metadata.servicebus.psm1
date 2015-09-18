Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

function Get-ServiceBusMetadata ($Context) {

	if (!$Context['UseCaseRoute']){
		return @{}
	}

	$useCaseRoute = $AllUseCaseRoutes | where { $Context.UseCaseRoute -contains $_ }
	$metadata = @{ 'UseCaseRoute' = $useCaseRoute }

	$metadata += @{
		'Routes' = @{
			'Default' = @{
				'SourceTopic' = 'topic.performedoperations'
				'SourceSubscription' = '9F2C5A2A-924C-485A-9790-9066631DB307'
				'DestTopic' = "topic.performedoperations.$($Context.EnvironmentName).import".ToLowerInvariant()
				'DestSubscription' = '9F2C5A2A-924C-485A-9790-9066631DB307'
			}
			'Production' = @{
				'SourceTopic' = 'topic.performedoperations.export'
				'SourceSubscription' = $Context.EnvironmentName.ToLowerInvariant()
				'DestTopic' = 'topic.performedoperations.production.russia.import'
				'DestSubscription' = '9F2C5A2A-924C-485A-9790-9066631DB307'
			}
		}
	}

	return @{ 'ServiceBus' = $metadata }
}

$AllUseCaseRoutes = @(
	'Default'
	'Production'
)

Export-ModuleMember -Function Get-ServiceBusMetadata