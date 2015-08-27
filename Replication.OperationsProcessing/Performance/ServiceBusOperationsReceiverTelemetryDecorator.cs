using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class ServiceBusOperationsReceiverTelemetryDecorator : IMessageReceiver
    {
        private readonly IMessageReceiver _receiver;

        public ServiceBusOperationsReceiverTelemetryDecorator(ServiceBusOperationsReceiver receiver)
        {
            _receiver = receiver;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            using (Probe.Create("Peek Erm Operations"))
            {
                return _receiver.Peek();
            }
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            using (Probe.Create("Complete Erm Operations"))
            {
                _receiver.Complete(successfullyProcessedMessages, failedProcessedMessages);
            }
        }
    }
}