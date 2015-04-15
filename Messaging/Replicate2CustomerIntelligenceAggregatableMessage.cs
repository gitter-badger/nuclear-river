using System;

using NuClear.AdvancedSearch.Messaging.Metadata.Flows;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;

namespace NuClear.AdvancedSearch.Messaging
{
    public sealed class Replicate2CustomerIntelligenceAggregatableMessage : MessageBase, IAggregatableMessage
    {
        private readonly Guid _id = Guid.NewGuid();

        public override Guid Id
        {
            get { return _id; }
        }

        public IMessageFlow TargetFlow
        {
            get { return Replicate2CustomerIntelligenceFlow.Instance; }
        }
    }
}