using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class ServiceBusOperationsReceiverTelemetryWrapper : IMessageReceiver
    {
        private readonly IMessageReceiver _receiver;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly ITracer _tracer;

        public ServiceBusOperationsReceiverTelemetryWrapper(ServiceBusOperationsReceiver receiver, ITelemetryPublisher telemetryPublisher, ITracer tracer)
        {
            _receiver = receiver;
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            using (Probe.Create("Peek Erm Operations"))
            {
                var messages = _receiver.Peek();
                var serviceBusMessageCount = messages.Cast<ServiceBusPerformedOperationsMessage>().Sum(x => x.Operations.Count());
                _telemetryPublisher.Publish<ErmReceivedUseCaseCountIdentity>(serviceBusMessageCount);

                var dublicates = messages.GroupBy(x => x.Id).Where(group => group.Count() > 1).ToArray();
                if (dublicates.Any())
                {
                    _tracer.Warn(string.Format("found message dublicates: {0}", string.Join(", ", dublicates.Select(x => x.Key.ToString()))));
                }

                return messages;
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