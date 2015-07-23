using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;

namespace NuClear.Replication.OperationsProcessing.Transports.CorporateBus
{
    public sealed class CorporateBusDtoMessage : IAggregatableMessage
    {
        public Guid Id { get; set; }
        public IMessageFlow TargetFlow { get; set; }

        public IReadOnlyCollection<ICorporateBusDto> Dtos { get; set; }

        public bool Equals(IMessage other)
        {
            return ReferenceEquals(this, other);
        }
    }
}