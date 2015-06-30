Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

# TODO Это временное решение, правильное решение - это создание отдельного сервиса инфраструктуры, общего для advanced search и для ERM

Import-Module "$PSScriptRoot\metadata.web.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.taskservice.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.transform.psm1" -DisableNameChecking

$EnvironmentMetadata = @{
	'Test.21' = @{
		'Replication.EntryPoint' = @{
			'TargetHosts' = @('uk-erm-test01')
		}
		'ConvertUseCasesService' = @{
			'TargetHosts' = @('uk-erm-test01')
		}
	}
	'Test.09' = @{
		'Replication.EntryPoint' = @{
			'TargetHosts' = @('uk-erm-test01')
		}
		'ConvertUseCasesService' = @{
			'TargetHosts' = @('uk-erm-test01')
		}
	}
}

function Get-EnvironmentMetadata ($EnvName, [ValidateSet('Test', 'Int', 'Load', 'Production', 'Edu', 'Business')]$EnvType, $Country, $Index) {

	return @{
		'Transform' = Get-TransformMetadata $EnvName $EnvType $Country $Index
		'Web.OData' = Get-WebMetadata $EnvType $Country 'Web.OData' $Index
		'Replication.EntryPoint' = Get-TaskServiceMetadata $EnvType $Country 'Replication.EntryPoint' $Index
		'ConvertUseCasesService' = Get-TaskServiceMetadata $EnvType $Country 'ConvertUseCasesService' $Index
	}
}

$EnvironmentMetadata = @{

	'Edu.Chile' = Get-EnvironmentMetadata 'Edu.Chile' 'Edu' 'Chile'
	'Edu.Cyprus' = Get-EnvironmentMetadata 'Edu.Cyprus' 'Edu' 'Cyprus'
	'Edu.Czech' = Get-EnvironmentMetadata 'Edu.Czech' 'Edu' 'Czech'
	'Edu.Emirates' = Get-EnvironmentMetadata 'Edu.Emirates' 'Edu' 'Emirates'
	'Edu.Kazakhstan' = Get-EnvironmentMetadata 'Edu.Kazakhstan' 'Edu' 'Kazakhstan'
	'Edu.Russia' = Get-EnvironmentMetadata 'Edu.Russia' 'Edu' 'Russia'
	'Edu.Ukraine' = Get-EnvironmentMetadata 'Edu.Ukraine' 'Edu' 'Ukraine'
	'Business.Russia' = Get-EnvironmentMetadata 'Business.Russia' 'Business' 'Russia'

	'Int.Chile' = Get-EnvironmentMetadata 'Int.Chile' 'Int' 'Chile'
	'Int.Cyprus' = Get-EnvironmentMetadata 'Int.Cyprus' 'Int' 'Cyprus'
	'Int.Czech' = Get-EnvironmentMetadata 'Int.Czech' 'Int' 'Czech'
	'Int.Emirates' = Get-EnvironmentMetadata 'Int.Emirates' 'Int' 'Emirates'
	'Int.Kazakhstan' = Get-EnvironmentMetadata 'Int.Kazakhstan' 'Int' 'Kazakhstan'
	'Int.Russia' = Get-EnvironmentMetadata 'Int.Russia' 'Int' 'Russia'
	'Int.Ukraine' = Get-EnvironmentMetadata 'Int.Ukraine' 'Int' 'Ukraine'
	
	'Load.Russia' = Get-EnvironmentMetadata 'Load.Russia' 'Load' 'Russia'
	'Load.Cyprus' = Get-EnvironmentMetadata 'Load.Cyprus' 'Load' 'Cyprus'
	'Load.Czech' = Get-EnvironmentMetadata 'Load.Czech' 'Load' 'Czech'
	'Load.Ukraine' = Get-EnvironmentMetadata 'Load.Ukraine' 'Load' 'Ukraine'

	'Production.Chile' = Get-EnvironmentMetadata 'Production.Chile' 'Production' 'Chile'
	'Production.Cyprus' = Get-EnvironmentMetadata 'Production.Cyprus' 'Production' 'Cyprus'
	'Production.Czech' = Get-EnvironmentMetadata 'Production.Czech' 'Production' 'Czech'
	'Production.Emirates' = Get-EnvironmentMetadata 'Production.Emirates' 'Production' 'Emirates'
	'Production.Kazakhstan' = Get-EnvironmentMetadata 'Production.Kazakhstan' 'Production' 'Kazakhstan'
	'Production.Russia' = Get-EnvironmentMetadata 'Production.Russia' 'Production' 'Russia'
	'Production.Ukraine' = Get-EnvironmentMetadata 'Production.Ukraine' 'Production' 'Ukraine'

	'Test.01' = Get-EnvironmentMetadata 'Test.01' 'Test' 'Russia' '01'
	'Test.02' = Get-EnvironmentMetadata 'Test.02' 'Test' 'Russia' '02'
	'Test.03' = Get-EnvironmentMetadata 'Test.03' 'Test' 'Russia' '03'
	'Test.04' = Get-EnvironmentMetadata 'Test.04' 'Test' 'Russia' '04'
	'Test.05' = Get-EnvironmentMetadata 'Test.05' 'Test' 'Russia' '05'
	'Test.06' = Get-EnvironmentMetadata 'Test.06' 'Test' 'Russia' '06'

	'Test.07' = Get-EnvironmentMetadata 'Test.07' 'Test' 'Russia' '07'
	'Test.08' = Get-EnvironmentMetadata 'Test.08' 'Test' 'Russia' '08'
	'Test.09' = Get-EnvironmentMetadata 'Test.09' 'Test' 'Russia' '09'
	'Test.10' = Get-EnvironmentMetadata 'Test.10' 'Test' 'Russia' '10'
	'Test.11' = Get-EnvironmentMetadata 'Test.11' 'Test' 'Russia' '11'
	'Test.12' = Get-EnvironmentMetadata 'Test.12' 'Test' 'Russia' '12'
	'Test.13' = Get-EnvironmentMetadata 'Test.13' 'Test' 'Russia' '13'
	'Test.14' = Get-EnvironmentMetadata 'Test.14' 'Test' 'Russia' '14'
	'Test.15' = Get-EnvironmentMetadata 'Test.15' 'Test' 'Russia' '15'
	'Test.16' = Get-EnvironmentMetadata 'Test.16' 'Test' 'Russia' '16'
	'Test.17' = Get-EnvironmentMetadata 'Test.17' 'Test' 'Russia' '17'
	'Test.18' = Get-EnvironmentMetadata 'Test.18' 'Test' 'Russia' '18'
	'Test.19' = Get-EnvironmentMetadata 'Test.19' 'Test' 'Russia' '19'
	'Test.20' = Get-EnvironmentMetadata 'Test.20' 'Test' 'Russia' '20'
	'Test.21' = Get-EnvironmentMetadata 'Test.21' 'Test' 'Russia' '21'
	'Test.22' = Get-EnvironmentMetadata 'Test.22' 'Test' 'Russia' '22'
	'Test.23' = Get-EnvironmentMetadata 'Test.23' 'Test' 'Russia' '23'
	'Test.24' = Get-EnvironmentMetadata 'Test.24' 'Test' 'Russia' '24'
	'Test.25' = Get-EnvironmentMetadata 'Test.25' 'Test' 'Russia' '25'
	'Test.88' = Get-EnvironmentMetadata 'Test.88' 'Test' 'Russia' '88'
	
	'Test.101' = Get-EnvironmentMetadata 'Test.101' 'Test' 'Cyprus' '101'
	'Test.102' = Get-EnvironmentMetadata 'Test.102' 'Test' 'Cyprus' '102'
	'Test.103' = Get-EnvironmentMetadata 'Test.103' 'Test' 'Cyprus' '103'
	'Test.108' = Get-EnvironmentMetadata 'Test.108' 'Test' 'Cyprus' '108'

	'Test.201' = Get-EnvironmentMetadata 'Test.201' 'Test' 'Czech' '201'
	'Test.202' = Get-EnvironmentMetadata 'Test.202' 'Test' 'Czech' '202'
	'Test.203' = Get-EnvironmentMetadata 'Test.203' 'Test' 'Czech' '203'
	'Test.208' = Get-EnvironmentMetadata 'Test.208' 'Test' 'Czech' '208'

	'Test.301' = Get-EnvironmentMetadata 'Test.301' 'Test' 'Chile' '301'
	'Test.302' = Get-EnvironmentMetadata 'Test.302' 'Test' 'Chile' '302'
	'Test.303' = Get-EnvironmentMetadata 'Test.303' 'Test' 'Chile' '303'
	'Test.304' = Get-EnvironmentMetadata 'Test.304' 'Test' 'Chile' '304'
	'Test.308' = Get-EnvironmentMetadata 'Test.308' 'Test' 'Chile' '308'
	'Test.320' = Get-EnvironmentMetadata 'Test.320' 'Test' 'Chile' '320'
	
	'Test.401' = Get-EnvironmentMetadata 'Test.401' 'Test' 'Ukraine' '401'
	'Test.402' = Get-EnvironmentMetadata 'Test.402' 'Test' 'Ukraine' '402'
    'Test.403' = Get-EnvironmentMetadata 'Test.403' 'Test' 'Ukraine' '403'
	'Test.404' = Get-EnvironmentMetadata 'Test.404' 'Test' 'Ukraine' '404'
	'Test.408' = Get-EnvironmentMetadata 'Test.408' 'Test' 'Ukraine' '408'

	'Test.501' = Get-EnvironmentMetadata 'Test.501' 'Test' 'Emirates' '501'
	'Test.502' = Get-EnvironmentMetadata 'Test.502' 'Test' 'Emirates' '502'
	'Test.503' = Get-EnvironmentMetadata 'Test.503' 'Test' 'Emirates' '503'
	'Test.508' = Get-EnvironmentMetadata 'Test.508' 'Test' 'Emirates' '508'

	'Test.601' = Get-EnvironmentMetadata 'Test.601' 'Test' 'Kazakhstan' '601'
	'Test.602' = Get-EnvironmentMetadata 'Test.602' 'Test' 'Kazakhstan' '602'
	'Test.603' = Get-EnvironmentMetadata 'Test.603' 'Test' 'Kazakhstan' '603'
    'Test.604' = Get-EnvironmentMetadata 'Test.604' 'Test' 'Kazakhstan' '604'
    'Test.605' = Get-EnvironmentMetadata 'Test.605' 'Test' 'Kazakhstan' '605'
	'Test.608' = Get-EnvironmentMetadata 'Test.608' 'Test' 'Kazakhstan' '608'
}

Export-ModuleMember -Variable EnvironmentMetadata