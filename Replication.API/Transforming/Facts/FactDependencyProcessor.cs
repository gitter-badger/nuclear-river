using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    internal class FactDependencyProcessor<TFact>
    {
        private readonly IFactDependencyInfo<TFact> _metadata;

        public FactDependencyProcessor(IFactDependencyInfo<TFact> metadata)
        {
            _metadata = metadata;
        }

        public IEnumerable<IOperation> ProcessCreation(IQuery query, IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(query, factIds, _metadata.CreationMappingSpecificationProvider);
        }

        public IEnumerable<IOperation> ProcessUpdating(IQuery query, IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(query, factIds, _metadata.UpdatingMappingSpecificationProvider);
        }

        public IEnumerable<IOperation> ProcessDeletion(IQuery query, IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(query, factIds, _metadata.DeletionMappingSpecificationProvider);
        }

        private IEnumerable<IOperation> ProcessDependencies(IQuery query, IReadOnlyCollection<long> factIds, MapToObjectsSpecProvider<TFact> operationFactory)
        {
            using (Probe.Create("Querying dependent aggregates"))
            {
                var filter = _metadata.FindSpecificationProvider.Invoke(factIds);
                return operationFactory.Invoke(filter).Map(query).Cast<IOperation>();
            }
        }
    }
}