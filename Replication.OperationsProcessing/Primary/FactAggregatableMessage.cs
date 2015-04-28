using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public class FactAggregatableMessage : IAggregatableMessage
    {
        public Guid Id { get; set; }
        public IMessageFlow TargetFlow { get; set; }

        public IEnumerable<FactOperation> Operations { get; set; }

        public bool Equals(IMessage other)
        {
            var otherFact = other as FactAggregatableMessage;
            return otherFact != null && Equals(Id, otherFact.Id);
        }
    }
}