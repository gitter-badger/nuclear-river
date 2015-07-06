using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public sealed class AggregateOperationAccumulator<TMessageFlow> : MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, AggregateOperationAggregatableMessage>
        where TMessageFlow: class, IMessageFlow, new()
    {
        private readonly ITelemetryPublisher _telemetryPublisher;

        public AggregateOperationAccumulator(ITelemetryPublisher telemetryPublisher)
        {
            _telemetryPublisher = telemetryPublisher;
        }

        protected override AggregateOperationAggregatableMessage Process(PerformedOperationsFinalProcessingMessage message)
        {
            _telemetryPublisher.Trace("Process", message);

            var operations = message.FinalProcessings.Select(x => x.OperationId.CreateOperation(x.EntityTypeId, x.EntityId)).ToList();

            return new AggregateOperationAggregatableMessage
            {
                TargetFlow = MessageFlow,
                Operations = operations
            };
        }
    }
}