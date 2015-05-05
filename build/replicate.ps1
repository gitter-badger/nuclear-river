param($LibDir=$null, $ErmConString=$null, $FactsConString=$null)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

function Load-Assemblies ($LibDir) {
	$files =  Get-ChildItem $LibDir -Filter '*.dll'
	foreach($file in $files) {
		$bytes = [System.IO.File]::ReadAllBytes($file.FullName)
		[void][System.Reflection.Assembly]::Load($bytes)
	}
}

Load-Assemblies $LibDir

$ermSchema = [NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]::Erm
$ermConnection = [LinqToDB.DataProvider.SqlServer.SqlServerTools]::CreateDataConnection($ErmConString).AddMappingSchema($ermSchema)
$ermContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.ErmContext($ermConnection)
$factTransformationContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.FactsTransformationContext($ermContext)

$factsSchema = [NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]::Facts
$factsConnection = [LinqToDB.DataProvider.SqlServer.SqlServerTools]::CreateDataConnection($FactsConString).AddMappingSchema($factsSchema)

$bulkCopyOptions = new-Object LinqToDB.Data.BulkCopyOptions
$bulkCopyOptions.BulkCopyTimeout = 0

$type = [NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.FactsTransformationContext]
$properties = $type.GetProperties()

foreach ($property in $properties){
	$queryable = $property.GetValue($factTransformationContext) -as [System.Linq.IQueryable]
	if ($queryable -eq $null){
		continue
	}

	Write-Host "$($property.Name)..."

	[void][LinqToDB.Data.DataConnectionExtensions]::BulkCopy($factsConnection, $bulkCopyOptions, $queryable)
}

"Done"