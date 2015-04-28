using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class FilteringStrategy : MessageProcessingStrategyBase<Replicate2CustomerIntelligenceFlow, TrackedUseCase, FactAggregatableMessage>
    {
        private readonly ErmOperationAdapter _adapter;

        public FilteringStrategy()
        {
            _adapter = new ErmOperationAdapter();
        }

        protected override FactAggregatableMessage Process(TrackedUseCase message)
        {
            var changes = message.Operations.SelectMany(scope => scope.ChangesContext.UntypedChanges)
                                 .SelectMany(type =>
                                             type.Value.SelectMany(change =>
                                                                   change.Value.Details.Select(detail =>
                                                                                               new ErmOperationAdapter.ErmOperation
                                                                                               {
                                                                                                   EntityType = type.Key.Id,
                                                                                                   EntityId = change.Value.Id,
                                                                                                   Change = (int)detail.ChangesType
                                                                                               })))
                                 .ToArray();

            return new FactAggregatableMessage
                   {
                       Id = Guid.NewGuid(),
                       TargetFlow = MessageFlow,
                       Operations = _adapter.Convert(changes),
                   };
        }
    }
}
