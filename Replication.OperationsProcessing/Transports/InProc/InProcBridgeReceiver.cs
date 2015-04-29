using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;

namespace NuClear.Replication.OperationsProcessing.Transports.InProc
{
    public class InProcBridgeReceiver : IMessageReceiver
    {
        public const int BatchSize = 10;

        // TODO {a.rechkalov, 23.04.2015}: Переместить очередь в БД
        public static readonly List<PerformedOperationFinalProcessing> MessageQueue = new List<PerformedOperationFinalProcessing>();

        public IReadOnlyList<IMessage> Peek()
        {
            return Query().Take(BatchSize).ToArray();
        }

        private IEnumerable<PerformedOperationsFinalProcessingMessage> Query()
        {
            return from operation in MessageQueue
                   group operation by new { operation.EntityId, operation.EntityTypeId }
                   into operationsGroup
                   let operationsGroupKey = operationsGroup.Key
                   let maxAttempt = operationsGroup.Max(processing => processing.AttemptCount)
                   orderby maxAttempt
                   select new PerformedOperationsFinalProcessingMessage
                          {
                              EntityId = operationsGroupKey.EntityId,
                              EntityName = EntityType.Instance.Parse(operationsGroupKey.EntityTypeId),
                              MaxAttemptCount = maxAttempt,
                              FinalProcessings = operationsGroup
                          };
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            throw new NotImplementedException();
        }
    }
}
