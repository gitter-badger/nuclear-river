using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Settings;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.Storage.Readings;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class ErmFactsTransformation
    {
        private readonly IQuery _query;
        private readonly ITracer _tracer;
        private readonly IReplicationSettings _replicationSettings;
        private readonly IFactProcessorFactory _factProcessorFactory;
        private readonly IDataChangesApplierFactory _dataChangesApplierFactory;
        private readonly IMetadataSource<IFactInfo> _metadataSource;

        public ErmFactsTransformation(
            IQuery query,
            IReplicationSettings replicationSettings,
            IFactProcessorFactory factProcessorFactory, 
            IDataChangesApplierFactory dataChangesApplierFactory,
            IMetadataSource<IFactInfo> metadataSource,
            ITracer tracer)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (factProcessorFactory == null)
            {
                throw new ArgumentNullException("factProcessorFactory");
            }

            _query = query;
            _tracer = tracer;
            _replicationSettings = replicationSettings;
            _factProcessorFactory = factProcessorFactory;
            _dataChangesApplierFactory = dataChangesApplierFactory;
            _metadataSource = metadataSource;
        }

        public IReadOnlyCollection<IOperation> Transform(IEnumerable<FactOperation> operations)
        {
            using (Probe.Create("ETL1 Transforming"))
            {
                var result = Enumerable.Empty<IOperation>();

                var slices = operations.GroupBy(operation => new { operation.FactType })
                                       .OrderByDescending(slice => slice.Key.FactType, new FactTypePriorityComparer());

                foreach (var slice in slices)
                {
                    var factType = slice.Key.FactType;
                    var factIds = slice.Select(x => x.FactId).Distinct();

                    IFactInfo factInfo;
                    if (!_metadataSource.Metadata.TryGetValue(factType, out factInfo))
                    {
                        throw new NotSupportedException(string.Format("The '{0}' fact not supported.", factType));
                    }

                    using (Probe.Create("ETL1 Transforming", factInfo.Type.Name))
                    {
                        foreach (var batch in factIds.CreateBatches(_replicationSettings.ReplicationBatchSize))
                        {
                            var processor = _factProcessorFactory.Create(factInfo);

                            _tracer.Debug("Apply changes to target facts storage");
                            var aggregateOperations = processor.ApplyChanges(_query, _dataChangesApplierFactory.Create(factInfo.Type), batch);

                            result = result.Concat(aggregateOperations);
                        }
                    }
                }

                return result.Distinct().ToList();
            }
        }
    }
}
