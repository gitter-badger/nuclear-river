using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using NuClear.AdvancedSearch.Replication.Bulk.Metamodel;
using NuClear.AdvancedSearch.Replication.Bulk.Model;
using NuClear.CustomerIntelligence.Domain;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
	public sealed class Program
	{
	    private static readonly MetadataProvider DefaultProvider
	        = new MetadataProvider(
	            new IMetadataSource[]
	            {
	                new MassReplicationMetadataSource(),
	                new FactsReplicationMetadataSource(),
	                new AggregateConstructionMetadataSource(),
	                new StatisticsRecalculationMetadataSource(),
	            },
	            new IMetadataProcessor[] { });

		public static void Main(string[] args)
		{
		    foreach (var replicationMetadataKey in args)
		    {
		        var actions = GetReplicationActions(replicationMetadataKey);
                Parallel.ForEach(actions, action => action.Item2.Invoke());
            }
		}

        public static IEnumerable<Tuple<string, Action>> GetReplicationActions(string key)
        {
            var element = GetMassReplicationMetadata(key);
            return GetReplicationMetadata(element.MetadataReference)
                .Select(metadata => Tuple.Create(metadata.Identity.Id.ToString(), new Action(() => Replicate(element, metadata))));
        }

        private static MassReplicationMetadataElement GetMassReplicationMetadata(string key)
        {
            IMetadataElement element;
            var id = Metadata.Id.For<MassReplicationMetadataKindIdentity>(key).Build().AsIdentity();
            DefaultProvider.TryGetMetadata(id.Id, out element);

            return (MassReplicationMetadataElement)element;
        }

        private static IEnumerable<IMetadataElement> GetReplicationMetadata(Uri uri)
	    {
            IMetadataElement replicationMetadata;
            DefaultProvider.TryGetMetadata(uri, out replicationMetadata);

            return ((HierarchyMetadata)replicationMetadata).Elements;
	    }

	    private static void Replicate(MassReplicationMetadataElement massReplicationMetadata, IMetadataElement concreteMetadata)
	    {
            using (var source = massReplicationMetadata.Source.CreateConnection())
            using (var target = massReplicationMetadata.Target.CreateConnection())
            {
                var sw = Stopwatch.StartNew();
                var sourceQuery = new LinqToDbQuery(source);
                var processor = massReplicationMetadata.Factory.Invoke(concreteMetadata, sourceQuery, target);
                processor.Process();
                sw.Stop();
                Console.WriteLine($"{concreteMetadata.Identity.Id}, {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
