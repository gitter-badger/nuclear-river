using System;

namespace NuClear.AdvancedSearch.Replication.API.Operations
{
    public sealed class InitializeAggregate : AggregateOperation
    {
        public InitializeAggregate(Type aggregateType, long aggregateId)
            : base(aggregateType, aggregateId)
        {
        }
    }
}