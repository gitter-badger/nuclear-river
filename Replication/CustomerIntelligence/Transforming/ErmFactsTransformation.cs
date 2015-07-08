using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class ErmFactsTransformation
    {
        private readonly IErmFactsContext _source;
        private readonly IErmFactsContext _target;
        private readonly IDataMapper _mapper;
        private readonly ITransactionManager _transactionManager;

        public ErmFactsTransformation(IErmFactsContext source, IErmFactsContext target, IDataMapper mapper, ITransactionManager transactionManager)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            _source = source;
            _target = target;
            _mapper = mapper;
            _transactionManager = transactionManager;
        }

        public IEnumerable<AggregateOperation> Transform(IEnumerable<FactOperation> operations)
        {
            using (var probe = new Probe("ETL1 Transforming"))
            {
                return _transactionManager.WithinTransaction(() => DoTransform(operations));
            }
        }

        private IEnumerable<AggregateOperation> DoTransform(IEnumerable<FactOperation> operations)
        {
            var result = Enumerable.Empty<AggregateOperation>();

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

                using (var probe = new Probe("ETL1 Transforming " + factInfo.FactType.Name))
                {
                    var changes = factInfo.DetectChangesWith(this, factIds);
                    var aggregateOperations = factInfo.ApplyChangesWith(this, changes);
                    result = result.Concat(aggregateOperations);
                }
            }

            return result;
        }

        internal MergeTool.MergeResult<long> DetectChanges<T>(Func<IErmFactsContext, IQueryable<T>> query)
            where T : IErmFactObject
        {
            var result = MergeTool.Merge<long>(
                query.Invoke(_source).Select(fact => fact.Id), 
                query.Invoke(_target).Select(fact => fact.Id));

            return result;
        }

        internal IEnumerable<AggregateOperation> ApplyChanges<T>(
            Func<IErmFactsContext, IEnumerable<long>, IQueryable<T>> query,
            IReadOnlyCollection<FactDependencyInfo> dependentAggregates,
            MergeTool.MergeResult<long> changes)
            where T : IErmFactObject
        {
            var idsToCreate = changes.Difference.ToArray();
            var idsToUpdate = changes.Intersection.ToArray();
            var idsToDelete = changes.Complement.ToArray();
            
            var createResult = CreateFact(idsToCreate, query, dependentAggregates);
            var updateResult = UpdateFact(idsToUpdate, query, dependentAggregates);
            var deleteResult = DeleteFact(idsToDelete, query, dependentAggregates);

            return createResult.Concat(updateResult).Concat(deleteResult);
        }

        private IEnumerable<AggregateOperation> CreateFact<T>(IReadOnlyCollection<long> factIds, Func<IErmFactsContext, IEnumerable<long>, IQueryable<T>> query, IReadOnlyCollection<FactDependencyInfo> dependentAggregates)
        {
            _mapper.InsertAll(query.Invoke(_source, factIds));

            return ProcessDependencies(dependentAggregates,
                                       factIds,
                                       (dependency, id) =>
                                       dependency.IsDirectDependency
                                           ? (AggregateOperation)new InitializeAggregate(dependency.AggregateType, id)
                                           : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id));
        }

        private IEnumerable<AggregateOperation> UpdateFact<T>(IReadOnlyCollection<long> factIds, Func<IErmFactsContext, IEnumerable<long>, IQueryable<T>> query, IReadOnlyCollection<FactDependencyInfo> dependentAggregates)
        {
            var before = ProcessDependencies(dependentAggregates.Where(x => !x.IsDirectDependency),
                                             factIds,
                                             (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id));

            _mapper.UpdateAll(query.Invoke(_source, factIds));

            var after = ProcessDependencies(dependentAggregates,
                                            factIds,
                                            (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id));

            return before.Concat(after);
        }

        private IEnumerable<AggregateOperation> DeleteFact<T>(IReadOnlyCollection<long> factIds, Func<IErmFactsContext, IEnumerable<long>, IQueryable<T>> query, IReadOnlyCollection<FactDependencyInfo> dependentAggregates)
        {
            var result = ProcessDependencies(dependentAggregates,
                                             factIds,
                                             (dependency, id) =>
                                             dependency.IsDirectDependency
                                                 ? (AggregateOperation)new DestroyAggregate(dependency.AggregateType, id)
                                                 : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id));

            _mapper.DeleteAll(query.Invoke(_target, factIds));

            return result;
        }

        private IReadOnlyCollection<AggregateOperation> ProcessDependencies(IEnumerable<FactDependencyInfo> dependencies, IEnumerable<long> ids, Func<FactDependencyInfo, long, AggregateOperation> build)
        {
            using (var probe = new Probe("Querying dependent aggregates"))
            {
                return dependencies.SelectMany(info => info.Query(_target, ids).Select(id => build(info, id))).ToArray();
            }
        }
    }
}
