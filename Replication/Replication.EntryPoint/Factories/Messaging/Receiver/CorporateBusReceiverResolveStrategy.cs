using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Receiver
{
    public sealed class CorporateBusReceiverResolveStrategy : MessageFlowReceiverResolveStrategyBase
    {
        public CorporateBusReceiverResolveStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(CorporateBusOperationsReceiver), metadata => metadata.IsPerformedOperationsPrimarySource() && Equals(metadata.MessageFlow, ImportFactsFromBitFlow.Instance))
        {
        }
    }
}