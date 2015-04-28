using System;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;

namespace NuClear.Replication.OperationsProcessing.Stages
{
    // TODO {a.rechkalov, 24.04.2015}: Не нужна в принципе, но нужна для корректной работы Primary.
    public sealed class EmptyStrategy : MessageProcessingStrategyBase<Replicate2CustomerIntelligenceFlow, IMessage, FactAggregatableMessage>
    {
        public EmptyStrategy()
        {
        }

        protected override FactAggregatableMessage Process(IMessage message)
        {
            return null;
        }
    }
}
