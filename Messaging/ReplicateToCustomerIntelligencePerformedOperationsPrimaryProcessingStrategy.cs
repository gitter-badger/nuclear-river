using NuClear.AdvancedSearch.Messaging.Metadata.Flows;
using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.OperationsTracking.API.UseCases;

namespace NuClear.AdvancedSearch.Messaging
{
    public sealed class ReplicateToCustomerIntelligencePerformedOperationsPrimaryProcessingStrategy :
        MessageProcessingStrategyBase<PrimaryReplicate2CustomerIntelligenceFlow, TrackedUseCase, Replicate2CustomerIntelligenceAggregatableMessage>
    {
        protected override Replicate2CustomerIntelligenceAggregatableMessage Process(TrackedUseCase message)
        {
            return new Replicate2CustomerIntelligenceAggregatableMessage();
        }
    }
}