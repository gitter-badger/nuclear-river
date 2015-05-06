param($LibDir, $ConnectionStrings)

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

# aliases
$sqlServerTools = [LinqToDB.DataProvider.SqlServer.SqlServerTools]
$schema = [NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema]

$ermConnection = $sqlServerTools::CreateDataConnection($ConnectionStrings.Erm).AddMappingSchema($schema::Erm)
$ermContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.ErmContext($ermConnection)
$factsTransformationContext = New-Object NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.FactsTransformationContext($ermContext)

$factsConnection = $sqlServerTools::CreateDataConnection($ConnectionStrings.Facts).AddMappingSchema($schema::Facts)

$bulkCopyOptions = new-Object LinqToDB.Data.BulkCopyOptions
$bulkCopyOptions.BulkCopyTimeout = 0

$properties = $factsTransformationContext.GetType().GetProperties()
foreach ($property in $properties){
	$queryable = $property.GetValue($factsTransformationContext) -as [System.Linq.IQueryable]
	if ($queryable -eq $null){
		continue
	}

	Write-Host "$($property.Name)..."

	[void][LinqToDB.Data.DataConnectionExtensions]::BulkCopy($factsConnection, $bulkCopyOptions, $queryable)
}

"Done"