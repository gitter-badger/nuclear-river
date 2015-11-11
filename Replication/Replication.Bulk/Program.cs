using System;
using System.Diagnostics;

using NuClear.CustomerIntelligence.Domain;
using NuClear.CustomerIntelligence.StateInitialization;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.Replication.Bulk
{
	public sealed class Program
	{
	    private static readonly MetadataProvider DefaultProvider
	        = new MetadataProvider(
	            new IMetadataSource[]
	            {
	                new BulkReplicationMetadataSource(),
	                new FactsReplicationMetadataSource(),
	                new AggregateConstructionMetadataSource(),
	                new StatisticsRecalculationMetadataSource(),
	            },
	            new IMetadataProcessor[] { });

		public static void Main(string[] args)
		{
		    var runner = new BulkReplicationRunner(DefaultProvider);
            foreach (var mode in args)
            {
                var sw = Stopwatch.StartNew();
                runner.Run(mode);
                Console.WriteLine($"{mode}, {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
