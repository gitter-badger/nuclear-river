using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ServiceBus.Messaging;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Replication.OperationsProcessing.Properties;

namespace NuClear.Replication.OperationsProcessing.Transport
{
    public sealed class MockServiceBusReceiver : IMessageReceiver
    {
        public IReadOnlyList<IMessage> Peek()
        {
            var brokeredMessage = new BrokeredMessage(new MemoryStream(Resources.UpdateFirm), true);
            brokeredMessage.Properties["UseCaseId"] = Guid.NewGuid();
            var message = new ServiceBusPerformedOperationsMessage(new[] { brokeredMessage });
            return new[] { message };
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
        }
    }
}
