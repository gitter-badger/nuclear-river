using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class CorporateBusAggregatableMessage : MessageBase, IAggregatableMessage
    {
        public override Guid Id { get { return Guid.Empty; } }
        public IMessageFlow TargetFlow { get; set; }

        public IReadOnlyCollection<ICorporateBusDto> Dtos { get; set; }
    }
}