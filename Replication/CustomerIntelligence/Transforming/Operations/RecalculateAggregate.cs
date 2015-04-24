using System;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations
{
    public sealed class RecalculateAggregate : AggregateOperation
    {
        public RecalculateAggregate(Type aggregateType, long aggregateId)
            : base(aggregateType, aggregateId)
        {
            Priority = 2;
        }
    }
}