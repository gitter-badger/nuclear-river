using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;

namespace NuClear.Replication.OperationsProcessing.Transport
{
    public class InternalReceiver : IMessageReceiver
    {
        // TODO {a.rechkalov, 23.04.2015}: Переместить очередь в БД
        public static readonly List<IMessage> MessageQueue = new List<IMessage>();

        public IReadOnlyList<IMessage> Peek()
        {
            return MessageQueue.ToArray();
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            foreach (var message in successfullyProcessedMessages)
            {
                MessageQueue.Remove(message);
            }
        }
    }
}
