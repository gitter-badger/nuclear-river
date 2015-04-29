using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;

namespace NuClear.Replication.OperationsProcessing.Transports.InProc
{
    public sealed class InProcBridgeSender
    {
        public void Push(IEnumerable<AggregateOperation> operations, IMessageFlow targetFlow)
        {
            var transportMessages = operations.Select(operation => SerializeMessage(operation, targetFlow));
            InProcBridgeReceiver.MessageQueue.AddRange(transportMessages);
        }

        private PerformedOperationFinalProcessing SerializeMessage(AggregateOperation operation, IMessageFlow targetFlow)
        {
            return new PerformedOperationFinalProcessing
                   {
                       CreatedOn = DateTime.UtcNow,
                       MessageFlowId = targetFlow.Id,
                       EntityId = operation.AggregateId,
                       EntityTypeId = 0, // FIXME {a.rechkalov, 29.04.2015}: Типам CI:: нужны идентификаторы или нужно использовать IEntityType
                   };
        }
    }
}
