using NuClear.Messaging.DI.Factories.Unity.Processors.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.Primary;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Tests.Mocks.Processor
{
    public sealed class PrimaryProcessorResolveStrategy : MessageFlowProcessorResolveStrategyBase
    {
        public PrimaryProcessorResolveStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(PerformedOperationsPrimaryFlowProcessor), PerformedOperations.IsPerformedOperationsPrimarySource)
        {
        }
    }
}