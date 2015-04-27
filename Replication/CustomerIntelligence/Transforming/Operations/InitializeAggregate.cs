using System;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations
{
    public sealed class InitializeAggregate : AggregateOperation
    {
        public InitializeAggregate(Type aggregateType, long aggregateId)
            : base(aggregateType, aggregateId)
        {
            Priority = 3;
        }
    }
}