using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class FactsTransformation
    {
        private readonly IQuery _source;
        private readonly IQuery _target;
        private readonly IDataMapper _mapper;

        public FactsTransformation(IQuery source, IQuery target, IDataMapper mapper)
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

            var slices = operations.GroupBy(operation => new { Operation = operation.GetType(), operation.FactType })
                                   .OrderByDescending(slice => slice.Key.Operation, new FactOperationPriorityComparer())
                                   .ThenByDescending(slice => slice.Key.FactType, new FactTypePriorityComparer());

            foreach (var slice in slices)
            {
                var operation = slice.Key.Operation;
                var factType = slice.Key.FactType;
                var factIds = slice.Select(x => x.FactId).ToArray();

                FactInfo factInfo;
                if (!Facts.TryGetValue(factType, out factInfo))
                {
                    throw new NotSupportedException(string.Format("The '{0}' fact not supported.", factType));
                }

                if (operation == typeof(CreateFact))
                {
                    result = result.Concat(CreateFact(factInfo, factIds));
                }
                
                if (operation == typeof(UpdateFact))
                {
                    result = result.Concat(UpdateFact(factInfo, factIds));
                }

                if (operation == typeof(DeleteFact))
                {
                    result = result.Concat(DeleteFact(factInfo, factIds));
                }
            }

            return result;
        }

        private IEnumerable<AggregateOperation> CreateFact(FactInfo info, long[] ids)
        {
            _mapper.InsertAll(info.Query(_source, ids));

            return ProcessDependencies(info.Aggregates, ids, (dependency, id) =>
                dependency.IsDirectDependency
                ? (AggregateOperation)new InitializeAggregate(dependency.AggregateType, id)
                : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id)).ToArray();
        }

        private IEnumerable<AggregateOperation> UpdateFact(FactInfo info, long[] ids)
        {
            IEnumerable<AggregateOperation> result = ProcessDependencies(info.Aggregates.Where(x => !x.IsDirectDependency), ids, 
                                                     (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id)).ToArray();

            _mapper.UpdateAll(info.Query(_source, ids));

            result = result.Concat(ProcessDependencies(info.Aggregates, ids, (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id)).ToArray());

            return result;
        }

        private IEnumerable<AggregateOperation> DeleteFact(FactInfo info, long[] ids)
        {
            var result = ProcessDependencies(info.Aggregates, ids, (dependency, id) =>
                dependency.IsDirectDependency
                ? (AggregateOperation)new DestroyAggregate(dependency.AggregateType, id)
                : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id)).ToArray();

            _mapper.DeleteAll(info.Query(_target, ids));

            return result;
        }

        private IEnumerable<AggregateOperation> ProcessDependencies(IEnumerable<FactDependencyInfo> dependencies, long[] ids, Func<FactDependencyInfo, long, AggregateOperation> build)
        {
            return dependencies.SelectMany(info => info.Query(_target, ids).Select(id => build(info, id)));
        }
    }
}
