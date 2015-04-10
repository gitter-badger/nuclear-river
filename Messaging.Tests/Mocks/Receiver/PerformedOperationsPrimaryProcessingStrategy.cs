using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;

namespace NuClear.AdvancedSearch.Messaging.Tests.Mocks.Receiver
{
    public sealed class PerformedOperationsPrimaryProcessingStrategy : MessageFlowReceiverResolveStrategyBase
    {
        public PerformedOperationsPrimaryProcessingStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(MockMessageReceiver), PerformedOperations.IsPerformedOperationsPrimarySource)
        {

        }
    }
}