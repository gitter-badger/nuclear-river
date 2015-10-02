using System;

namespace NuClear.AdvancedSearch.Common.Metadata.Model.Operations
{
    public sealed class InitializeAggregate : AggregateOperation
    {
        public InitializeAggregate(Type aggregateType, long aggregateId)
            : base(aggregateType, aggregateId)
        {
        }
    }
}