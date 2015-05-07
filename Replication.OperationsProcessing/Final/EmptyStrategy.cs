using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Primary;

namespace NuClear.Replication.OperationsProcessing.Final
{
    // TODO {a.rechkalov, 24.04.2015}: Не нужна в принципе, но нужна для корректной работы
    public sealed class EmptyStrategy
        : MessageProcessingStrategyBase<Replicate2CustomerIntelligenceFlow, AggregateOperationAggregatableMessage, AggregateOperationAggregatableMessage>
    {
        protected override AggregateOperationAggregatableMessage Process(AggregateOperationAggregatableMessage message)
        {
            return message;
        }
    }
}
