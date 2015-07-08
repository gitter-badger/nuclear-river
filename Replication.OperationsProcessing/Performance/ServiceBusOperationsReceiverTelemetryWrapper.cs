using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class ServiceBusOperationsReceiverTelemetryWrapper : IMessageReceiver
    {
        private readonly IMessageReceiver _receiver;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ServiceBusOperationsReceiverTelemetryWrapper(ServiceBusOperationsReceiver receiver, ITelemetryPublisher telemetryPublisher)
        {
            _receiver = receiver;
            _telemetryPublisher = telemetryPublisher;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            return _receiver.Peek();
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            var enqueuedTime = successfullyProcessedMessages
                .Concat(failedProcessedMessages)
                .Cast<ServiceBusPerformedOperationsMessage>()
                .SelectMany(message => message.Operations)
                .Select(message => message.EnqueuedTimeUtc)
                .Min();

            _telemetryPublisher.Publish<PrimaryProcessingDelayIdentity>((long)(DateTime.UtcNow - enqueuedTime).TotalMilliseconds);

            _receiver.Complete(successfullyProcessedMessages, failedProcessedMessages);
        }
    }
}