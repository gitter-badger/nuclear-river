using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API;
using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Settings;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class ErmFactsTransformation
    {
        private readonly MapSpecification<IEnumerable, IEnumerable<long>> _changesDetectionMapSpec
            = new MapSpecification<IEnumerable, IEnumerable<long>>(x => x.Cast<IIdentifiable>().Select(y => y.Id));

        private readonly ITracer _tracer;
        private readonly IReplicationSettings _replicationSettings;
        private readonly IQuery _query;
        private readonly IFactChangesApplierFactory _factChangesApplierFactory;

        public ErmFactsTransformation(ITracer tracer, IReplicationSettings replicationSettings, IQuery query, IFactChangesApplierFactory factChangesApplierFactory)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (factChangesApplierFactory == null)
            {
                throw new ArgumentNullException("factChangesApplierFactory");
            }

            _tracer = tracer;
            _replicationSettings = replicationSettings;
            _query = query;
            _factChangesApplierFactory = factChangesApplierFactory;
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
                    if (!ErmFactsTransformationMetadata.Facts.TryGetValue(factType, out factInfo))
                    {
                        throw new NotSupportedException(string.Format("The '{0}' fact not supported.", factType));
                    }

                    using (Probe.Create("ETL1 Transforming", factInfo.Type.Name))
                    {
                        foreach (var batch in factIds.CreateBatches(_replicationSettings.ReplicationBatchSize))
                        {
                            var changesDetector = new DataChangesDetector(factInfo, _query);
                            var statisticsOperationsDetector = new StatisticsOperationsDetector(factInfo, _query);
                            var changesApplier = _factChangesApplierFactory.Create(factInfo, _query);

                            _tracer.DebugFormat("Detecting changes between source and target storages for entity type '{0}'", factType.Name);
                            var changes = changesDetector.DetectChanges(_changesDetectionMapSpec, batch);

                            _tracer.Debug("Detecting statistics before changes applying");
                            var statisticsOperationsBeforeChanges = statisticsOperationsDetector.DetectOperations(batch);

                            _tracer.Debug("Apply changes to target facts storage");
                            var aggregateOperations = changesApplier.ApplyChanges(changes);

                            _tracer.Debug("Detecting statistics before changes applying");
                            var statisticsOperationsAfterChanges = statisticsOperationsDetector.DetectOperations(batch);

                            result = result.Concat(statisticsOperationsBeforeChanges)
                                           .Concat(aggregateOperations)
                                           .Concat(statisticsOperationsAfterChanges);
                        }
                    }
                }

                return result.Distinct().ToArray();
            }
        }
    }
}
