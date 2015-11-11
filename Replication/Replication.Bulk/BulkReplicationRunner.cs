using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Bulk.Factories;
using NuClear.Replication.Bulk.Metadata;
using NuClear.Replication.Bulk.Storage;

namespace NuClear.Replication.Bulk
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
            BulkReplicationMetadataElement bulkReplicationMetadata;
            var id = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<BulkReplicationMetadataKindIdentity>(mode).Build().AsIdentity();
            if (!_metadataProvider.TryGetMetadata(id.Id, out bulkReplicationMetadata))
            {
                throw new NotSupportedException("Bulk replication metadata cannot be found");
            }

            var storageDescriptor = bulkReplicationMetadata.Features.OfType<StorageDescriptorFeature>().Single(x => x.Direction == ReplicationDirection.To);
            var context = bulkReplicationMetadata.Elements.Single();
            using (ViewContainer.TemporaryRemoveViews(storageDescriptor.ConnectionStringName, bulkReplicationMetadata.EssentialViews))
            {
                Parallel.ForEach(context.Elements,
                                 element =>
                                 {
                                     using (var bulkReplicatorFactory = RoutingBulkReplicatorFactory.Create(bulkReplicationMetadata))
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