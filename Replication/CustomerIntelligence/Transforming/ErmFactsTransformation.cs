using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class ErmFactsTransformation
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

        public IEnumerable<AggregateOperation> Transform(IEnumerable<FactOperation> operations)
        {
            return _transactionManager.WithinTransaction(() => DoTransform(operations));
        }

        private IEnumerable<AggregateOperation> DoTransform(IEnumerable<FactOperation> operations)
        {
            var result = new List<AggregateOperation>();

            var slices = operations.GroupBy(operation => new { operation.FactType })
                                   .OrderByDescending(slice => slice.Key.FactType, new FactTypePriorityComparer());

            foreach (var slice in slices)
            {
                var factType = slice.Key.FactType;
                var factIds = slice.Select(x => x.FactId).Distinct().ToArray();

                ErmFactInfo factInfo;
                if (!Facts.TryGetValue(factType, out factInfo))
                {
                    throw new NotSupportedException(string.Format("The '{0}' fact not supported.", factType));
                }

                var changesDetector = CreateChangesDetector(factInfo);
                var changes = changesDetector.DetectChanges(factIds);

                var changesApplier = CreateChangesApplier(factInfo);
                var aggregateOperations = changesApplier.ApplyChanges(changes);

                result.AddRange(aggregateOperations);
            }

            return result;
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
