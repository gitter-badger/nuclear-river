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
            var sourceData = query.Invoke(_source, factIds).ToDictionary(fact => fact.Id);
            var targetData = query.Invoke(_target, factIds).ToDictionary(fact => fact.Id);

            var dataToCreate = sourceData.Where(x => !targetData.ContainsKey(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            var dataToUpdate = sourceData.Where(x => targetData.ContainsKey(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            var dataToDelete = targetData.Where(x => !sourceData.ContainsKey(x.Key)).ToDictionary(x => x.Key, x => x.Value);

            return CreateFact(dataToCreate, dependentAggregates)
                .Concat(UpdateFact(dataToUpdate, dependentAggregates))
                .Concat(DeleteFact(dataToDelete, dependentAggregates));
        }

        private IEnumerable<AggregateOperation> CreateFact<T>(IDictionary<long, T> data, IReadOnlyCollection<FactDependencyInfo> dependentAggregates)
        {
            if (!data.Any())
            {
                return Enumerable.Empty<AggregateOperation>();
            }

            foreach (var record in data.Values)
            {
                _mapper.Insert(record);
            }

            return ProcessDependencies(dependentAggregates,
                                       data.Keys,
                                       (dependency, id) =>
                                       dependency.IsDirectDependency
                                           ? (AggregateOperation)new InitializeAggregate(dependency.AggregateType, id)
                                           : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id));
        }

        private IEnumerable<AggregateOperation> UpdateFact<T>(IDictionary<long, T> data, IReadOnlyCollection<FactDependencyInfo> dependentAggregates)
        {
            if (!data.Any())
            {
                return Enumerable.Empty<AggregateOperation>();
            }

            var before = ProcessDependencies(dependentAggregates.Where(x => !x.IsDirectDependency),
                                             data.Keys,
                                             (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id));

            foreach (var record in data.Values)
            {
                _mapper.Update(record);
            }

            var after = ProcessDependencies(dependentAggregates,
                                            data.Keys,
                                            (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id));

            return before.Concat(after);
        }

        private IEnumerable<AggregateOperation> DeleteFact<T>(IDictionary<long, T> data, IReadOnlyCollection<FactDependencyInfo> dependentAggregates)
        {
            if (!data.Any())
            {
                return Enumerable.Empty<AggregateOperation>();
            }

            var result = ProcessDependencies(dependentAggregates,
                                             data.Keys,
                                             (dependency, id) =>
                                             dependency.IsDirectDependency
                                                 ? (AggregateOperation)new DestroyAggregate(dependency.AggregateType, id)
                                                 : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id));

            foreach (var record in data.Values)
            {
                _mapper.Delete(record);
            }

            return result;
        }

        private IEnumerable<AggregateOperation> ProcessDependencies(IEnumerable<FactDependencyInfo> dependencies, IEnumerable<long> ids, Func<FactDependencyInfo, long, AggregateOperation> build)
        {
            return dependencies.SelectMany(info => info.Query(_target, ids).Select(id => build(info, id))).ToArray();
        }
    }
}
