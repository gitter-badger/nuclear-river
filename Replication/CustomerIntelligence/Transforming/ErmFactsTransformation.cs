using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.Storage.Readings;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class ErmFactsTransformation
    {
        private readonly IQuery _sourceQuery;
        private readonly IQuery _destQuery;
        private readonly ISourceChangesDetectorFactory _sourceChangesDetectorFactory;
        private readonly ISourceChangesApplierFactory _sourceChangesApplierFactory;

        public ErmFactsTransformation(
            IQuery sourceQuery,
            IQuery destQuery,
            ISourceChangesDetectorFactory sourceChangesDetectorFactory,
            ISourceChangesApplierFactory sourceChangesApplierFactory)
        {
            if (sourceQuery == null)
            {
                throw new ArgumentNullException("sourceQuery");
            }

            if (destQuery == null)
            {
                throw new ArgumentNullException("destQuery");
            }

            if (sourceChangesDetectorFactory == null)
            {
                throw new ArgumentNullException("sourceChangesDetectorFactory");
            }

            if (sourceChangesApplierFactory == null)
            {
                throw new ArgumentNullException("sourceChangesApplierFactory");
            }

            _sourceQuery = sourceQuery;
            _destQuery = destQuery;
            _sourceChangesDetectorFactory = sourceChangesDetectorFactory;
            _sourceChangesApplierFactory = sourceChangesApplierFactory;
        }

        public IReadOnlyCollection<IOperation> Transform(IEnumerable<FactOperation> operations)
        {
            using (Probe.Create("ETL1 Transforming"))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                {
                    var result = Enumerable.Empty<IOperation>();

                    var slices = operations.GroupBy(operation => new { operation.FactType })
                                           .OrderByDescending(slice => slice.Key.FactType, new FactTypePriorityComparer());

                    foreach (var slice in slices)
                    {
                        var factType = slice.Key.FactType;
                        var factIds = slice.Select(x => x.FactId).Distinct().ToArray();

                        IFactInfo factInfo;
                        if (!ErmFactsTransformationMetadata.Facts.TryGetValue(factType, out factInfo))
                        {
                            throw new NotSupportedException(string.Format("The '{0}' fact not supported.", factType));
                        }

                        using (Probe.Create("ETL1 Transforming", factInfo.Type.Name))
                        {
                            var changesDetector = _sourceChangesDetectorFactory.Create(factInfo, _sourceQuery, _destQuery);
                            var changesApplier = _sourceChangesApplierFactory.Create(factInfo, _sourceQuery, _destQuery);
                            var statisticsOperationsDetector = new StatisticsOperationsDetector(factInfo, _destQuery);

                            var changes = changesDetector.DetectChanges(factIds);

                            var statisticsOperationsBeforeChanges = statisticsOperationsDetector.DetectOperations(factIds);
                            var aggregateOperations = changesApplier.ApplyChanges(changes);
                            var statisticsOperationsAfterChanges = statisticsOperationsDetector.DetectOperations(factIds);

                            result = result.Union(statisticsOperationsBeforeChanges)
                                           .Union(aggregateOperations)
                                           .Union(statisticsOperationsAfterChanges);
                        }
                    }

                    transaction.Complete();
                    return result.ToList();
                }
            }
        }
    }
}
