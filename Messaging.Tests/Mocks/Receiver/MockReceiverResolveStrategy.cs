using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;

namespace NuClear.AdvancedSearch.Messaging.Tests.Mocks.Receiver
{
    public sealed class MockReceiverResolveStrategy : MessageFlowReceiverResolveStrategyBase
    {
        public MockReceiverResolveStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(MockMessageReceiver), PerformedOperations.IsPerformedOperationsPrimarySource)
        {

        }
    }
}