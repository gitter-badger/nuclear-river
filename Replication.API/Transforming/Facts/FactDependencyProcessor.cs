using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public class FactDependencyProcessor<TFact> : IFactDependencyProcessor
    {
        private readonly IQuery _query;
        private readonly IFactDependencyInfo<TFact> _metadata;

        public FactDependencyProcessor(IQuery query, IFactDependencyInfo<TFact> metadata)
        {
            _query = query;
            _metadata = metadata;
        }

        public IEnumerable<IOperation> ProcessCreation(IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(factIds, _metadata.CreationMappingSpecificationProvider);
        }

        public IEnumerable<IOperation> ProcessUpdating(IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(factIds, _metadata.UpdatingMappingSpecificationProvider);
        }

        public IEnumerable<IOperation> ProcessDeletion(IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(factIds, _metadata.DeletionMappingSpecificationProvider);
        }

        private IEnumerable<IOperation> ProcessDependencies(IReadOnlyCollection<long> factIds, MapToObjectsSpecProvider<TFact, IOperation> operationFactory)
        {
            using (Probe.Create("Querying dependent aggregates"))
            {
                var filter = _metadata.FindSpecificationProvider.Invoke(factIds);
                return operationFactory.Invoke(filter).Map(_query);
            }
        }
    }
}