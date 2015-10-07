using System;

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
			using (var source = context.Source.CreateConnection())
			using (var target = context.Target.CreateConnection())
			{
                var sourceQuery = new LinqToDbQuery(source);
				foreach (var metadata in context.Metadata)
				{
					Console.WriteLine($"{metadata.Identity}");
					var processor = context.Factory.Invoke(metadata, sourceQuery, target);
					processor.Process();
				}
			}
	    }
    }
}
