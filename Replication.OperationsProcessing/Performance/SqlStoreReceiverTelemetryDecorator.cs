using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class SqlStoreReceiverTelemetryDecorator : IMessageReceiver
    {
        private readonly IMessageReceiver _receiver;

        public SqlStoreReceiverTelemetryDecorator(SqlStoreReceiver receiver)
        {
            _receiver = receiver;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            using (Probe.Create("Peek Aggregate Operations"))
            {
                return _receiver.Peek();
            }
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            using (Probe.Create("Complete Aggregate Operations"))
            {
                _receiver.Complete(successfullyProcessedMessages, failedProcessedMessages);
            }
        }
    }
}