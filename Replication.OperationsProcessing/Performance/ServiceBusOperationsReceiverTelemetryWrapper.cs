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
        private readonly ITelemetry _telemetry;

        public ServiceBusOperationsReceiverTelemetryWrapper(ServiceBusOperationsReceiver receiver, ITelemetry telemetry)
        {
            _receiver = receiver;
            _telemetry = telemetry;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            return _receiver.Peek();
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            var enqueuedTime = successfullyProcessedMessages
                .Concat(failedProcessedMessages)
                .OfType<ServiceBusPerformedOperationsMessage>()
                .SelectMany(message => message.Operations)
                .Select(message => message.EnqueuedTimeUtc)
                .Min();

            _telemetry.Report<PrimaryProcessingDelayIdentity>((long)(DateTime.UtcNow - enqueuedTime).TotalMilliseconds);

            _receiver.Complete(successfullyProcessedMessages, failedProcessedMessages);
        }
    }
}