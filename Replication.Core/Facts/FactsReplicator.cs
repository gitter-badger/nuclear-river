using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.API.Settings;
using NuClear.Replication.Metadata;
using NuClear.Replication.Metadata.Model;
using NuClear.Replication.Metadata.Operations;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace NuClear.Replication.Core.Facts
{
    public sealed class FactsReplicator : IFactsReplicator
    {
        private readonly ITracer _tracer;
        private readonly IReplicationSettings _replicationSettings;
        private readonly IFactProcessorFactory _factProcessorFactory;
        private readonly IMetadataProvider _metadataProvider;

        public FactsReplicator(
            IMetadataProvider metadataProvider,
            IFactProcessorFactory factProcessorFactory,
            IReplicationSettings replicationSettings,
            ITracer tracer)
        {
            _metadataProvider = metadataProvider;
            _tracer = tracer;
            _replicationSettings = replicationSettings;
            _factProcessorFactory = factProcessorFactory;
        }

        public IReadOnlyCollection<IOperation> Replicate(IEnumerable<FactOperation> operations, IComparer<Type> factTypePriorityComparer)
        {
            using (Probe.Create("ETL1 Transforming"))
            {
                MetadataSet metadataSet;
                if (!_metadataProvider.TryGetMetadata<ReplicationMetadataIdentity>(out metadataSet))
                {
                    throw new NotSupportedException(string.Format("Metadata for identity '{0}' cannot be found.", typeof(ReplicationMetadataIdentity).Name));
                }

                var result = Enumerable.Empty<IOperation>();

                var slices = operations.GroupBy(operation => new { operation.FactType })
                                       .OrderByDescending(slice => slice.Key.FactType, factTypePriorityComparer);

                foreach (var slice in slices)
                {
                    var factType = slice.Key.FactType;

                    IMetadataElement factMetadata;
                    Uri factMetadataId = MetadataBuilder.Id.For(ReplicationMetadataIdentity.Instance.Id, "Facts", factType.Name);
                    if (!metadataSet.Metadata.TryGetValue(factMetadataId, out factMetadata))
                    {
                        throw new NotSupportedException(string.Format("The fact of type '{0}' is not supported.", factType));
                    }

                    var factIds = slice.Select(x => x.FactId).Distinct();
                    using (Probe.Create("ETL1 Transforming", factType.Name))
                    {
                        var processor = _factProcessorFactory.Create(factType, factMetadata);

                        foreach (var batch in factIds.CreateBatches(_replicationSettings.ReplicationBatchSize))
                        {
                            _tracer.Debug("Apply changes to target facts storage");
                            var aggregateOperations = processor.ApplyChanges(batch);

                            result = result.Concat(aggregateOperations);
                        }
                    }
                }

                return result.Distinct().ToArray();
            }
        }
    }
}
