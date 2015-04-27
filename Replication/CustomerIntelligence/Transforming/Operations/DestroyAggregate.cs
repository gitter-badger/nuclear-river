using System;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations
{
    public sealed class DestroyAggregate : AggregateOperation
    {
        public DestroyAggregate(Type aggregateType, long aggregateId)
            : base(aggregateType, aggregateId)
        {
            Priority = 1;
        }
    }
}