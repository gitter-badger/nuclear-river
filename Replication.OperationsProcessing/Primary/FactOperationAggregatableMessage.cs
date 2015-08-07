using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public class FactOperationAggregatableMessage : IAggregatableMessage
    {
        public Guid Id { get; set; }
        public IMessageFlow TargetFlow { get; set; }

        public IReadOnlyCollection<FactOperation> Operations { get; set; }

        public bool Equals(IMessage other)
        {
            return ReferenceEquals(this, other);
        }
    }
}