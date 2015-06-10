using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class ErmFactsTransformation
    {
        private readonly IErmFactsContext _source;
        private readonly IErmFactsContext _target;
        private readonly IDataMapper _mapper;

        public ErmFactsTransformation(IErmFactsContext source, IErmFactsContext target, IDataMapper mapper)
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
        }

        public IEnumerable<AggregateOperation> Transform(IEnumerable<FactOperation> operations)
        {
            var result = Enumerable.Empty<AggregateOperation>();

            var slices = operations.GroupBy(operation => new { operation.FactType })
                                   .OrderByDescending(slice => slice.Key.FactType, new FactTypePriorityComparer());

            foreach (var slice in slices)
            {
                var factType = slice.Key.FactType;
                var factIds = slice.Select(x => x.FactId).ToArray();

                ErmFactInfo factInfo;
                if (!Facts.TryGetValue(factType, out factInfo))
                {
                    throw new NotSupportedException(string.Format("The '{0}' fact not supported.", factType));
                }

                result = result.Concat(factInfo.ApplyTo(this, factIds));
            }

            return result;
        }

        internal IEnumerable<AggregateOperation> Transform<T>(
            Func<IErmFactsContext, IEnumerable<long>, IQueryable<T>> query,
            IReadOnlyCollection<FactDependencyInfo> dependentAggregates,
            IReadOnlyCollection<long> factIds)
            where T : IErmFactObject
        {
            var sourceData = new HashSet<long>(query.Invoke(_source, factIds).Select(fact => fact.Id));
            var targetData = new HashSet<long>(query.Invoke(_target, factIds).Select(fact => fact.Id));

            var idsToCreate = sourceData.Where(x => !targetData.Contains(x)).ToArray();
            var idsToUpdate = sourceData.Where(x => targetData.Contains(x)).ToArray();
            var idsToDelete = targetData.Where(x => !sourceData.Contains(x)).ToArray();

            var createResult = idsToCreate.Any() ? CreateFact(idsToCreate, query, dependentAggregates) : Enumerable.Empty<AggregateOperation>();
            var updateResult = idsToUpdate.Any() ? UpdateFact(idsToUpdate, query, dependentAggregates) : Enumerable.Empty<AggregateOperation>();
            var deleteResult = idsToDelete.Any() ? DeleteFact(idsToDelete, query, dependentAggregates) : Enumerable.Empty<AggregateOperation>();

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

        private IEnumerable<AggregateOperation> ProcessDependencies(IEnumerable<FactDependencyInfo> dependencies, IEnumerable<long> ids, Func<FactDependencyInfo, long, AggregateOperation> build)
        {
            return dependencies.SelectMany(info => info.Query(_target, ids).Select(id => build(info, id))).ToArray();
        }
    }
}
