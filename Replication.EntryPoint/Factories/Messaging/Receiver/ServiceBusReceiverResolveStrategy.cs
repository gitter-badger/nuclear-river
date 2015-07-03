using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Receiver
{
    public sealed class ServiceBusReceiverResolveStrategy : MessageFlowReceiverResolveStrategyBase
    {
        public ServiceBusReceiverResolveStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(ServiceBusOperationsReceiver), metadata => metadata.IsPerformedOperationsPrimarySource() && Equals(metadata.MessageFlow, ImportFactsFromErmFlow.Instance))
        {
        }
    }
}