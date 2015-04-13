using System.Collections.Generic;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.API.Final;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;

namespace NuClear.Replication.OperationsProcessing.Final.Transports
{
    public class FinalProcessingQueueReceiver : MessageReceiverBase<PerformedOperationsFinalProcessingMessage, IFinalProcessingQueueReceiverSettings>
    {
        public FinalProcessingQueueReceiver(
            MessageFlowMetadata sourceFlowMetadata, 
            IFinalProcessingQueueReceiverSettings messageReceiverSettings) 
            : base(sourceFlowMetadata, messageReceiverSettings)
        {
        }

        protected override IReadOnlyList<PerformedOperationsFinalProcessingMessage> Peek()
        {
            throw new System.NotImplementedException();
        }

        protected override void Complete(
            IEnumerable<PerformedOperationsFinalProcessingMessage> successfullyProcessedMessages,
            IEnumerable<PerformedOperationsFinalProcessingMessage> failedProcessedMessages)
        {
            throw new System.NotImplementedException();
        }
    }
}