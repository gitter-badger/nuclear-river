using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;

namespace NuClear.Replication.OperationsProcessing.Stages
{
    public sealed class FilteringStrategy : MessageProcessingStrategyBase<Replicate2CustomerIntelligenceFlow, TrackedUseCase, FactOperation>
    {
        public FilteringStrategy()
        {
        }

        protected override FactOperation Process(TrackedUseCase message)
        {
            var changes = message.Operations.SelectMany(scope => scope.ChangesContext.UntypedChanges)
                                 .SelectMany(type =>
                                             type.Value.SelectMany(change =>
                                                                   change.Value.Details.Select(detail =>
                                                                                               new
                                                                                               {
                                                                                                   TypeId = type.Key.Id,
                                                                                                   EntityId = change.Value.Id,
                                                                                                   Change = detail.ChangesType
                                                                                               })))
                                 .ToArray();

            // TODO {a.rechkalov, 23.04.2015}: Реализовать фильтрацию
            return new FactOperation
                   {
                       Id = Guid.NewGuid(),
                       TargetFlow = MessageFlow,
                   };
        }
    }
}
