using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Transports;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public sealed class AggregateOperationAccumulator<TMessageFlow> : MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, AggregateOperationAggregatableMessage>
        where TMessageFlow: class, IMessageFlow, new()
    {
        protected override AggregateOperationAggregatableMessage Process(PerformedOperationsFinalProcessingMessage message)
        {
            var operations = message.FinalProcessings.Select(x => x.OperationId.CreateOperation(x.EntityTypeId, x.EntityId));

            return new AggregateOperationAggregatableMessage
            {
                TargetFlow = MessageFlow,
                Operations = operations
            };
        }
    }
}