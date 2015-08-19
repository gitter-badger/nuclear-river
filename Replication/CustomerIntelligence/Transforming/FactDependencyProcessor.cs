using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.Storage.Readings;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal class FactDependencyProcessor
    {
         private static readonly Func<IFactDependencyInfo, long, AggregateOperation> OperationsFactoryOnCreateFact =
            (dependency, id) =>
            dependency.IsDirectDependency
                ? (AggregateOperation)new InitializeAggregate(dependency.AggregateType, id)
                : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id);

        private static readonly Func<IFactDependencyInfo, long, AggregateOperation> OperationsFactoryOnUpdateFact =
            (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id);

        private static readonly Func<IFactDependencyInfo, long, AggregateOperation> OperationsFactoryOnDeleteFact =
            (dependency, id) =>
            dependency.IsDirectDependency
                ? (AggregateOperation)new DestroyAggregate(dependency.AggregateType, id)
                : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id);

        private readonly IQuery _query;
        private readonly IEnumerable<IFactDependencyInfo> _dependencies;

        public FactDependencyProcessor(IQuery query, IEnumerable<IFactDependencyInfo> dependencies)
        {
            _query = query;
            _dependencies = dependencies;
        }

        public IEnumerable<AggregateOperation> ProcessOnCreateFact(IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(factIds, OperationsFactoryOnCreateFact);
        }

        public IEnumerable<AggregateOperation> ProcessOnUpdateFact(IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(factIds, OperationsFactoryOnUpdateFact);
        }

        public IEnumerable<AggregateOperation> ProcessOnDeleteFact(IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(factIds, OperationsFactoryOnDeleteFact);
        }

        private IEnumerable<AggregateOperation> ProcessDependencies(IReadOnlyCollection<long> factIds, Func<IFactDependencyInfo, long, AggregateOperation> operationFactory)
        {
            var aggregateOperations = new List<AggregateOperation>();
            foreach (var dependency in _dependencies)
            {
                var mapSpec = dependency.MapToDependentAggregateSpecProvider(factIds);

                IEnumerable<long> dependencyIds;
                using (Probe.Create("Querying dependent aggregates"))
                {
                    dependencyIds = mapSpec.Map(_query).ToArray();
                }

                aggregateOperations.AddRange(dependencyIds.Select(dependencyId => operationFactory(dependency, dependencyId)));
            }

            return aggregateOperations;
        }
    }
}