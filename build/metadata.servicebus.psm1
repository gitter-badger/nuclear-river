Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

$SharedAccessKeys = @{
	'Production.Russia' = 'XPnmzI+uyfZtKpUo4Ys79Lr8cmlLftTjd5jxeYMAp0I='
	'Load.Russia' = 'daOQn6EYUWbJPlfV1592+xCIQRTumxZzzEw2c1i1u0M='
	'Test.01' = 'dxorbZPNw1/uiwnp86NfFOgRaJqe3IMzFfMLTG3omM4='
	'Test.02' = '2t4kdp1F87nw01Gkjgy7s7gzblYCHgt6lzn/emeNNg8='
	'Test.03' = '68Av4IZXBAKVu2fpUntiGx8Jliz7V9N4T9aYRRW/olM='
	'Test.04' = '+HRJRuoNqYB+UI+b36zQOFqckofYKwbhHjDLRL+zi0c='
	'Test.05' =	'0XGFXdeOOgu2ANkrw+2cZ1AabmEIhDvNbxvNd0m5kIQ='
	'Test.06' =	'Vx7wWYOw6QEZl579WRC/NF+x1xiZ23rzpF1SIWbc2BY='
	'Test.07' =	'jpsOJzkgZIav7cnkRdHRwLKBVFaAtA3fru8zjQZ5aBU='
	'Test.08' =	'uXqO99MHiKRDNHQbpnjgOpwVfx135vq+GnoDYdzmAPc='
	'Test.09' =	'GZ2fkxahg+Y6F2rnTIYzWAkTL4+SA5oLsaBkZUg6nM8='
	'Test.10' =	'IjQmMo0FcE3X/bAVqRQLt+AxGp5eV8dXrRZQ1VejX4U='
	'Test.11' =	'JnsMI9pS2ASYmjcZgY4whBZTDD9A03s+oGsBnxGcgzU='
	'Test.12' =	'b39/VUn2gDM1QyvE85MfU4IhPYuXhVYkt/gm3R+8q0o='
	'Test.13' =	'pltsZ1EsJbO6iF03331zkhlZyTeJgjxkyzc+rtDqn30='
	'Test.14' =	'3NtvvXk7stOFs9SbgewXEXGfeZxZY2MClr8Fv6/ojso='
	'Test.15' =	'S9NbpkmdOZJvTKH3GtQ0C1GCIjnIt23l93OMQolLfyk='
	'Test.16' =	'ozi4uSJtbjB26+tNh4wLedj8G0WuKjQuel4A/vAL2KQ='
	'Test.17' =	'7hIIZhmUg+4v231LdlaCrvpeGAdPWm4BqTHiNeMC81I='
	'Test.18' =	'0+Zrovn/TuTWvZZxJuNeXQS55R0mndtzXoHsH7S/BHQ='
	'Test.19' =	'N6Glgu9S34dBtdG+yyhP1qDX3TUsLxCGkNbWzWN7SuU='
	'Test.20' =	'RL8HERGWm3/ZQTLpJ27tPhqq+2hb3HcyN7CttB/9F2U='
	'Test.21' =	'fWDhmeSQ9ss+Pp44oUirEizlzEg/oBBOxCaBCsspCLo='
}

function Get-SharedAccessKeyMetadata($EnvName){

	if (!$SharedAccessKeys.ContainsKey($EnvName)){
		return @{}
	}

	return @{ '{SharedAccessKey}' = $SharedAccessKeys[$EnvName] }
}

Export-ModuleMember -Function Get-SharedAccessKeyMetadata