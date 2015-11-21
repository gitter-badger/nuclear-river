using System;

namespace NuClear.AdvancedSearch.Common.Metadata.Model.Operations
{
    public sealed class DestroyAggregate : AggregateOperation
    {
        public DestroyAggregate(Type aggregateType, long aggregateId)
            : base(aggregateType, aggregateId)
        {
        }
    }
}