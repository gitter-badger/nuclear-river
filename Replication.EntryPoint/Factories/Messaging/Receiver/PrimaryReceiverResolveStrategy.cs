using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Transports;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Receiver
{
    public sealed class PrimaryReceiverResolveStrategy : MessageFlowReceiverResolveStrategyBase
    {
        public PrimaryReceiverResolveStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(ServiceBusImitationReceiver), PerformedOperations.IsPerformedOperationsPrimarySource)
        {
        }
    }
}