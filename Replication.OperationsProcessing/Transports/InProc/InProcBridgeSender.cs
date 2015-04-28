using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;

namespace NuClear.Replication.OperationsProcessing.Transports.InProc
{
    public sealed class InProcBridgeSender
    {
        public void Push(ReplicationMessage<AggregateOperation> message)
        {
            InProcBridgeReceiver.MessageQueue.Add(message);
        }
    }
}
