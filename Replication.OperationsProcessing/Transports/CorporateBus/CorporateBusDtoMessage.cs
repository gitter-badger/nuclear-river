using System;
using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;

namespace NuClear.Replication.OperationsProcessing.Transports.CorporateBus
{
    public sealed class CorporateBusDtoMessage : IAggregatableMessage
    {
        public Guid Id { get; set; }
        public IMessageFlow TargetFlow { get; set; }

        public IEnumerable<object> Dtos { get; set; }
        public bool Equals(IMessage other)
        {
            return ReferenceEquals(this, other);
        }
    }
}