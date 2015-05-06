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
$ermContext = [NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.ErmContext]
$factsTransformationContext = [NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation.FactsTransformationContext]

$ermConnection = $sqlServerTools::CreateDataConnection($ConnectionStrings.Erm).AddMappingSchema($schema::Erm)
$ermContext = New-Object $ermContext($ermConnection)
$factTransformationContext = New-Object $factsTransformationContext($ermContext)

$factsConnection = $sqlServerTools::CreateDataConnection($ConnectionStrings.Facts).AddMappingSchema($schema::Facts)

$bulkCopyOptions = new-Object LinqToDB.Data.BulkCopyOptions
$bulkCopyOptions.BulkCopyTimeout = 0

$properties = $factsTransformationContext.GetProperties()
foreach ($property in $properties){
	$queryable = $property.GetValue($factTransformationContext) -as [System.Linq.IQueryable]
	if ($queryable -eq $null){
		continue
	}

	Write-Host "$($property.Name)..."

	[void][LinqToDB.Data.DataConnectionExtensions]::BulkCopy($factsConnection, $bulkCopyOptions, $queryable)
}

"Done"