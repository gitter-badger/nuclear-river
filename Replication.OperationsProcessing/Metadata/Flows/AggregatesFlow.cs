using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.Replication.OperationsProcessing.Metadata.Flows
{
    public sealed class AggregatesFlow : MessageFlowBase<AggregatesFlow>
    {
        public override Guid Id
        {
            get { return new Guid("96F17B1A-4CC8-40CC-9A92-16D87733C39F"); }
        }

        public override string Description
        {
            get { return ""; }
        }
    }
}