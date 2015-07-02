using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Performance;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Receiver
{
    public sealed class ServiceBusReceiverResolveStrategy : MessageFlowReceiverResolveStrategyBase
    {
        public ServiceBusReceiverResolveStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(ServiceBusOperationsReceiverTelemetryWrapper), metadata => metadata.IsPerformedOperationsPrimarySource() && Equals(metadata.MessageFlow, ImportFactsFromErmFlow.Instance))
        {
        }
    }
}