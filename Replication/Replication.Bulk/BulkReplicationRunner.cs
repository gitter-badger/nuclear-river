using System.Threading.Tasks;

using NuClear.AdvancedSearch.Replication.Bulk.Metamodel;
using NuClear.AdvancedSearch.Replication.Bulk.Processors;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
    public sealed class BulkReplicationRunner
    {
        private readonly IMetadataProvider _metadataProvider;

        public BulkReplicationRunner(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public void Run(string mode)
        {
            IMetadataElement bulkReplicationMetadata;
            var id = Metadata.Id.For<BulkReplicationMetadataKindIdentity>(mode).Build().AsIdentity();
            _metadataProvider.TryGetMetadata(id.Id, out bulkReplicationMetadata);

            Parallel.ForEach(bulkReplicationMetadata.Elements,
                             element =>
                             {
                                 var bulkReplicatorFactory = RoutingBulkReplicatorFactory.Create(element);
                                 var replicators = bulkReplicatorFactory.Create(element);
                                 foreach (var replicator in replicators)
                                 {
                                     replicator.Replicate();
                                 }
                             });
        }
    }
}