using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;

namespace NuClear.Replication.OperationsProcessing.Transports.InProc
{
    public class InProcBridgeReceiver : IMessageReceiver
    {
        // TODO {a.rechkalov, 23.04.2015}: Переместить очередь в БД
        public static readonly List<ReplicationMessage<AggregateOperation>> MessageQueue = new List<ReplicationMessage<AggregateOperation>>();

        public IReadOnlyList<IMessage> Peek()
        {
            return MessageQueue.ToArray();
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            throw new NotImplementedException();
        }
    }
}
