﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.API.Settings;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

namespace NuClear.Replication.Core.Facts
{
    public sealed class FactsReplicator : IFactsReplicator
    {
        private readonly ITracer _tracer;
        private readonly IReplicationSettings _replicationSettings;
        private readonly IMetadataProvider _metadataProvider;
        private readonly IFactProcessorFactory _factProcessorFactory;
        private readonly IComparer<Type> _factTypePriorityComparer;

        public FactsReplicator(
            ITracer tracer,
            IReplicationSettings replicationSettings,
            IMetadataProvider metadataProvider,
            IFactProcessorFactory factProcessorFactory,
            IComparer<Type> factTypePriorityComparer)
        {
            _tracer = tracer;
            _replicationSettings = replicationSettings;
            _metadataProvider = metadataProvider;
            _factProcessorFactory = factProcessorFactory;
            _factTypePriorityComparer = factTypePriorityComparer;
        }

        public IReadOnlyCollection<IOperation> Replicate(IEnumerable<FactOperation> operations)
        {
            using (Probe.Create("ETL1 Transforming"))
            {
                var result = Enumerable.Empty<IOperation>();

                var slices = operations.GroupBy(operation => new { operation.FactType })
                                       .OrderByDescending(slice => slice.Key.FactType, _factTypePriorityComparer);

                foreach (var slice in slices)
                {
                    var factType = slice.Key.FactType;

                    IMetadataElement factMetadata;
                    var metadataId = ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri(string.Format("Facts/{0}", factType.Name), UriKind.Relative));
                    if (!_metadataProvider.TryGetMetadata(metadataId, out factMetadata))
                    {
                        throw new NotSupportedException(string.Format("The fact of type '{0}' is not supported.", factType));
                    }

                    var factIds = slice.Select(x => x.FactId).Distinct();
                    using (Probe.Create("ETL1 Transforming", factType.Name))
                    {
                        var processor = _factProcessorFactory.Create(factMetadata);

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
