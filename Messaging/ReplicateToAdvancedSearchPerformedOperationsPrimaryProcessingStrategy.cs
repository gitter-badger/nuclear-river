using NuClear.AdvancedSearch.Messaging.Metadata.Flows;
using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.OperationsTracking.API.UseCases;

namespace NuClear.AdvancedSearch.Messaging
{
    public sealed class ReplicateToAdvancedSearchPerformedOperationsPrimaryProcessingStrategy :
        MessageProcessingStrategyBase<Replicate2AdvancedSearchFlow, TrackedUseCase, Replicate2AdvancedSearchAggregatableMessage>
    {
        protected override Replicate2AdvancedSearchAggregatableMessage Process(TrackedUseCase message)
        {
            return new Replicate2AdvancedSearchAggregatableMessage();
        }
    }
}