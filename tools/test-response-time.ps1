param (
  [string] $file
)
function read-file {
    param (
        [string]$fileName
    )

    return Get-Content $fileName | where { -not $_.StartsWith("#") }
}

function apply-projects {
    param (
        [Parameter(ValueFromPipeline=$True)]
        [string]$uri,
        [string[]]$projects
    )

    process {
        return $projects | foreach { $uri.Replace('{ProjectId}', $_) }
    }
}

function construct-uri {
    param (
        [Parameter(ValueFromPipeline=$True)]
        [string]$relativeUri,
        [string]$connectionstring
    )

    process {
        $builder = New-Object System.UriBuilder($connectionstring)
        $builder.Path = $relativeUri.Split('?')[0]
        $builder.Query = $relativeUri.Split('?')[1]

        return $builder.Uri
    }
}

function test-uri {
    param (
        [Parameter(ValueFromPipeline=$True)]
        [System.Uri]$uri
    )

    process {
        try {
            #warm up
            for($i=0; $i -lt 3; $i++) {
                $x = invoke-webrequest -uri $uri
            }
        
            #measure
            $invokeCount = 5
            $sw = [system.diagnostics.stopwatch]::startnew()    
            for($i=0; $i -lt $invokeCount; $i++) {
                $x = invoke-webrequest -uri $uri
            }
            $sw.stop();
            return [System.Tuple]::Create($uri, [string]$x.StatusCode, [double]$sw.elapsedmilliseconds / $invokeCount)
        }
        catch {
            return [System.Tuple]::Create($uri, $_.Exception.Message, [double]0)
        }
    }
}

$localhost = "http://localhost:15591/"
$production = "https://search.api.prod.erm.2gis.ru"
$test21 = "http://search21.api.test.erm.2gis.ru"
$projects = 10,4,8,129

read-file -fileName $file | apply-projects -projects $projects | construct-uri -connectionstring $production | test-uri | Out-GridView
