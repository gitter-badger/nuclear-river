using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.ServiceBus.Messaging;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Replication.OperationsProcessing.Properties;

namespace NuClear.Replication.OperationsProcessing.Transports.InProc
{
    public sealed class ServiceBusImitationReceiver : IMessageReceiver
    {
        public IReadOnlyList<IMessage> Peek()
        {
            var list = new List<IMessage>();
            for (var i = 0; i < 3; i++)
            {
                var brokeredMessage = new BrokeredMessage(new MemoryStream(Resources.UpdateFirm), true);
                brokeredMessage.Properties["UseCaseId"] = Guid.NewGuid();
                var message = new ServiceBusPerformedOperationsMessage(new[] { brokeredMessage });
                list.Add(message);
            }

            return list;
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
        }
    }
}
