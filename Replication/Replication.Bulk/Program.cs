using System;
using System.Diagnostics;
using System.Threading.Tasks;

using NuClear.Metamodeling.Elements;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
	public sealed class Program
    {
		public static void Main(string[] args)
		{
			var options = Options.Parse(args);

			if (options.Facts)
			{
				Replicate(Context.ErmToFacts);
			}

			if (options.CustomerIntelligence)
			{
				Replicate(Context.FactsToCustomerIntelligence);
			}

			if (options.Statistics)
			{
				Replicate(Context.FactsToCustomerIntelligenceStatistics);
			}
		}

	    private static void Replicate(MassReplicationContext context)
	    {
			Console.WriteLine($"{context.Source.ConnectionStringName} -> {context.Target.ConnectionStringName}");
	        using (ViewContainer.TemporaryRemoveViews(context.Target, context.EssentialViews))
	        {
                Parallel.ForEach(context.Metadata, metadata => Replicate(context, metadata));
            }
        }

	    private static void Replicate(MassReplicationContext context, IMetadataElement metadata)
	    {
            using (var source = context.Source.CreateConnection())
            using (var target = context.Target.CreateConnection())
            {
                var sw = Stopwatch.StartNew();
                var sourceQuery = new LinqToDbQuery(source);
                var processor = context.Factory.Invoke(metadata, sourceQuery, target);
                processor.Process();
                sw.Stop();
                Console.WriteLine($"{metadata.Identity.Id}, {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
