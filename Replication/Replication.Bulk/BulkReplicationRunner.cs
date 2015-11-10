using System;
using System.Linq;
using System.Threading.Tasks;

using NuClear.AdvancedSearch.Replication.Bulk.Factories;
using NuClear.AdvancedSearch.Replication.Bulk.Metadata;
using NuClear.AdvancedSearch.Replication.Bulk.Storage;
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
            IMetadataElement metadataElement;
            var id = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<BulkReplicationMetadataKindIdentity>(mode).Build().AsIdentity();
            if (!_metadataProvider.TryGetMetadata(id.Id, out metadataElement))
            {
                throw new NotSupportedException("Bulk replication metadata cannot be found");
            }

            var bulkReplicationMetadata = (BulkReplicationMetadataElement)metadataElement;

            var storageDescriptor = bulkReplicationMetadata.Features.OfType<StorageDescriptorFeature>().Single(x => x.Direction == ReplicationDirection.To);
            using (ViewContainer.TemporaryRemoveViews(storageDescriptor.ConnectionStringName, new[] { bulkReplicationMetadata.EssentialView }))
            {
                Parallel.ForEach(metadataElement.Elements,
                                 element =>
                                 {
                                     using (var bulkReplicatorFactory = RoutingBulkReplicatorFactory.Create(element))
                                     {
                                         var replicators = bulkReplicatorFactory.Create(element);
                                         foreach (var replicator in replicators)
                                         {
                                             replicator.Replicate();
                                         }
                                     }
                                 });
            }
        }
    }
}