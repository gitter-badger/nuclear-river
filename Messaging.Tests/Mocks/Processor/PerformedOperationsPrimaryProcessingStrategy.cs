using NuClear.Messaging.DI.Factories.Unity.Processors.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.Primary;

namespace NuClear.AdvancedSearch.Messaging.Tests.Mocks.Processor
{
    public sealed class PerformedOperationsPrimaryProcessingStrategy : MessageFlowProcessorResolveStrategyBase
    {
        public PerformedOperationsPrimaryProcessingStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(PerformedOperationsPrimaryFlowProcessor), PerformedOperations.IsPerformedOperationsPrimarySource)
        {
        }
    }
}