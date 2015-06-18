using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class ServiceBusOperationsReceiverWrapper : IMessageReceiver
    {
        private readonly IMessageReceiver _receiver;
        private readonly IProfiler _profiler;

        public ServiceBusOperationsReceiverWrapper(ServiceBusOperationsReceiver receiver, IProfiler profiler)
        {
            _receiver = receiver;
            _profiler = profiler;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            return _receiver.Peek();
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            var enqueuedTime = successfullyProcessedMessages.OfType<ServiceBusPerformedOperationsMessage>()
                                                            .SelectMany(message => message.Operations)
                                                            .Select(message => message.EnqueuedTimeUtc)
                                                            .Min();

            _profiler.Report<PrimaryProcessingDelayIdentity>((long)(DateTime.UtcNow - enqueuedTime).TotalMilliseconds);

            _receiver.Complete(successfullyProcessedMessages, failedProcessedMessages);
        }
    }
}