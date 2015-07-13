using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public class AggregateOperationAggregatableMessage : IAggregatableMessage
    {
        public Guid Id { get; set; }
        public IMessageFlow TargetFlow { get; set; }

        public IEnumerable<AggregateOperation> Operations { get; set; }

        public bool Equals(IMessage other)
        {
            return ReferenceEquals(this, other);
        }
    }
}