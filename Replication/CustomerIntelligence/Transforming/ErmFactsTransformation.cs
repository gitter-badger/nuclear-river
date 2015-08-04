using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Transforming;
using NuClear.Storage.Readings;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class ErmFactsTransformation
    {
        private readonly IQuery _sourceQuery;
        private readonly IQuery _destQuery;
        private readonly IDataMapper _mapper;
        private readonly ITransactionManager _transactionManager;

        public ErmFactsTransformation(IQuery sourceQuery, IQuery destQuery, IDataMapper mapper, ITransactionManager transactionManager)
        {
            if (sourceQuery == null)
            {
                throw new ArgumentNullException("sourceQuery");
            }

            if (destQuery == null)
            {
                throw new ArgumentNullException("destQuery");
            }

            _sourceQuery = sourceQuery;
            _destQuery = destQuery;
            _mapper = mapper;
            _transactionManager = transactionManager;
        }

        public IReadOnlyCollection<IOperation> Transform(IEnumerable<FactOperation> operations)
        {
            using (Probe.Create("ETL1 Transforming"))
            {
                return _transactionManager.WithinTransaction(() => DoTransform(operations));
            }
        }

        private IReadOnlyCollection<IOperation> DoTransform(IEnumerable<FactOperation> operations)
        {
            var result = Enumerable.Empty<IOperation>();

            var slices = operations.GroupBy(operation => new { operation.FactType })
                                   .OrderByDescending(slice => slice.Key.FactType, new FactTypePriorityComparer());

            foreach (var slice in slices)
            {
                var factType = slice.Key.FactType;
                var factIds = slice.Select(x => x.FactId).Distinct().ToArray();

                ErmFactInfo factInfo;
                if (!ErmFactsTransformationMetadata.Facts.TryGetValue(factType, out factInfo))
                {
                    throw new NotSupportedException(string.Format("The '{0}' fact not supported.", factType));
                }

                using (Probe.Create("ETL1 Transforming", factInfo.FactType.Name))
                {
                    var statisticsOperationsDetector = new StatisticsOperationsDetector(factInfo, _destQuery);
                    var changesDetector = CreateChangesDetector(factInfo);
                    var changesApplier = CreateChangesApplier(factInfo);

                    var changes = changesDetector.DetectChanges(factIds);

                    var statisticsOperationsBeforeChanges = statisticsOperationsDetector.DetectOperations(factIds);
                    var aggregateOperations = changesApplier.ApplyChanges(changes);
                    var statisticsOperationsAfterChanges = statisticsOperationsDetector.DetectOperations(factIds);

                    result = result.Union(statisticsOperationsBeforeChanges)
                                   .Union(aggregateOperations)
                                   .Union(statisticsOperationsAfterChanges);
                }
            }

            return result.ToList();
        }

        private IChangesDetector CreateChangesDetector(ErmFactInfo factInfo)
        {
            return (IChangesDetector)Activator.CreateInstance(typeof(ChangesDetector<>).MakeGenericType(factInfo.FactType), factInfo, _sourceQuery, _destQuery);
        }

        private IChangesApplier CreateChangesApplier(ErmFactInfo factInfo)
        {
            return (IChangesApplier)Activator.CreateInstance(typeof(ChangesApplier<>).MakeGenericType(factInfo.FactType), factInfo, _sourceQuery, _destQuery, _mapper);
        }
    }
}
