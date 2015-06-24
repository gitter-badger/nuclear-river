using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class ErmFactsTransformation
    {
        private readonly IQuery _ermQuery;
        private readonly IQuery _factsQuery;
        private readonly IDataMapper _mapper;

        public ErmFactsTransformation(IQuery ermQuery, IQuery factsQuery, IDataMapper mapper)
        {
            if (ermQuery == null)
            {
                throw new ArgumentNullException("ermQuery");
            }

            if (factsQuery == null)
            {
                throw new ArgumentNullException("factsQuery");
            }

            _ermQuery = ermQuery;
            _factsQuery = factsQuery;
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

                var changes = factInfo.DetectChangesWith(this, factIds);
                var aggregateOperations = factInfo.ApplyChangesWith(this, changes);
                result = result.Concat(aggregateOperations);
            }

            return result;
        }

        internal MergeTool.MergeResult<long> DetectChanges<T>(Func<IQuery, IQueryable<T>> ermQueryFunc, Func<IQuery, IQueryable<T>> factsQueryFunc)
            where T : IErmFactObject
        {
            var result = MergeTool.Merge<long>(
                ermQueryFunc.Invoke(_ermQuery).Select(fact => fact.Id),
                factsQueryFunc.Invoke(_factsQuery).Select(fact => fact.Id));

            return result;
        }

        internal IEnumerable<AggregateOperation> ApplyChanges<T>(
            Func<IQuery, IEnumerable<long>, IQueryable<T>> query,
            IReadOnlyCollection<FactDependencyInfo> dependentAggregates,
            MergeTool.MergeResult<long> changes)
            where T : class, IErmFactObject
        {
            var idsToCreate = changes.Difference.ToArray();
            var idsToUpdate = changes.Intersection.ToArray();
            var idsToDelete = changes.Complement.ToArray();
            
            var createResult = CreateFact(idsToCreate, query, dependentAggregates);
            var updateResult = UpdateFact(idsToUpdate, query, dependentAggregates);
            var deleteResult = DeleteFact(idsToDelete, query, dependentAggregates);

            return createResult.Concat(updateResult).Concat(deleteResult);
        }

        private IEnumerable<AggregateOperation> CreateFact<T>(IReadOnlyCollection<long> factIds, Func<IQuery, IEnumerable<long>, IQueryable<T>> query, IReadOnlyCollection<FactDependencyInfo> dependentAggregates)
        {
            _mapper.InsertAll<T>(query.Invoke(_ermQuery, factIds));

            return ProcessDependencies(dependentAggregates,
                                       factIds,
                                       (dependency, id) =>
                                       dependency.IsDirectDependency
                                           ? (AggregateOperation)new InitializeAggregate(dependency.AggregateType, id)
                                           : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id));
        }

        private IEnumerable<AggregateOperation> UpdateFact<T>(IReadOnlyCollection<long> factIds, Func<IQuery, IEnumerable<long>, IQueryable<T>> query, IReadOnlyCollection<FactDependencyInfo> dependentAggregates)
        {
            var before = ProcessDependencies(dependentAggregates.Where(x => !x.IsDirectDependency),
                                             factIds,
                                             (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id));

            _mapper.UpdateAll(query.Invoke(_ermQuery, factIds));

            var after = ProcessDependencies(dependentAggregates,
                                            factIds,
                                            (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id));

            return before.Concat(after);
        }

        private IEnumerable<AggregateOperation> DeleteFact<T>(IReadOnlyCollection<long> factIds, Func<IQuery, IEnumerable<long>, IQueryable<T>> query, IReadOnlyCollection<FactDependencyInfo> dependentAggregates) 
            where T : class, IErmFactObject
        {
            var result = ProcessDependencies(dependentAggregates,
                                             factIds,
                                             (dependency, id) =>
                                             dependency.IsDirectDependency
                                                 ? (AggregateOperation)new DestroyAggregate(dependency.AggregateType, id)
                                                 : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id));

            _mapper.DeleteAll(_factsQuery.For(new FindSpecification<T>(x => factIds.Contains(x.Id))));

            return result;
        }

        private IReadOnlyCollection<AggregateOperation> ProcessDependencies(IEnumerable<FactDependencyInfo> dependencies, IEnumerable<long> ids, Func<FactDependencyInfo, long, AggregateOperation> build)
        {
            return dependencies.SelectMany(info => info.Query(_factsQuery, ids).Select(id => build(info, id))).ToArray();
        }
    }
}
