using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Operations.Identities;
using NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates;
using NuClear.Aggregates;

namespace NuClear.AdvancedSearch.Replication.API.AggregateServices
{
    public interface IInitializeAggregateService<TAggregate> : IUnknownAggregateOperationService<InitializeAggregateOperationIdentity>
    {
        void Initialize(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds);
    }
}