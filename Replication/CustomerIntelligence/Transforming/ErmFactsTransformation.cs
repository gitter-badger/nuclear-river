using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API;
using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class ErmFactsTransformation
    {
        private readonly IQuery _sourceQuery;
        private readonly IQuery _destQuery;
        private readonly IFactChangesApplierFactory _factChangesApplierFactory;
        private readonly MapSpecification<IEnumerable, IEnumerable<long>> _changesDetectionMapSpec; 

        public ErmFactsTransformation(
            IQuery sourceQuery,
            IQuery destQuery,
            IFactChangesApplierFactory factChangesApplierFactory)
        {
            if (sourceQuery == null)
            {
                throw new ArgumentNullException("sourceQuery");
            }

            if (destQuery == null)
            {
                throw new ArgumentNullException("destQuery");
            }

            if (factChangesApplierFactory == null)
            {
                throw new ArgumentNullException("factChangesApplierFactory");
            }

            _sourceQuery = sourceQuery;
            _destQuery = destQuery;
            _factChangesApplierFactory = factChangesApplierFactory;

            _changesDetectionMapSpec = new MapSpecification<IEnumerable, IEnumerable<long>>(x => x.Cast<IIdentifiable>().Select(y => y.Id));
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
                            var changesDetector = new SourceChangesDetector(factInfo, _sourceQuery, _destQuery);
                            var statisticsOperationsDetector = new StatisticsOperationsDetector(factInfo, _destQuery);
                            var changesApplier = _factChangesApplierFactory.Create(factInfo, _sourceQuery, _destQuery);
                            
                            var changes = changesDetector.DetectChanges(_changesDetectionMapSpec, factIds);

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
