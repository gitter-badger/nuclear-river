using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NuClear.Messaging.API;

namespace NuClear.Replication.OperationsProcessing.Transport
{
    public sealed class InternalSender
    {
        // TODO {a.rechkalov, 23.04.2015}: Переместить очередь в БД
        public void Push(IEnumerable<IMessage> messages)
        {
            InternalReceiver.MessageQueue.AddRange(messages);
        }
    }
}
