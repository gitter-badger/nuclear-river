using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.Model.Common.Entities;
using NuClear.OperationsTracking.API.Changes;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Metadata.Model.Context;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    /// <summary>
    /// Стратегия выполняет фильтрацию операций, приехавших в TUC, и преобразование этих операций в операции над фактами.
    /// </summary>
    public sealed class PerformedOperationsFilteringStrategy :
        MessageProcessingStrategyBase<Replicate2CustomerIntelligenceFlow, TrackedUseCase, FactOperationAggregatableMessage>
    {
        private readonly ITracer _tracer;

        public PerformedOperationsFilteringStrategy(ITracer tracer)
        {
            _tracer = tracer;
        }

        protected override FactOperationAggregatableMessage Process(TrackedUseCase message)
        {
            _tracer.DebugFormat("Processing TUC {0}", message.Id);

            var plainChanges =
                message.Operations.SelectMany(scope => scope.ChangesContext.UntypedChanges)
                       .SelectMany(
                           x => x.Value.SelectMany(
                               y => y.Value.Details.Select(
                                   z => new ErmOperation(x.Key, y.Key, z.ChangesType))));

            return new FactOperationAggregatableMessage
                   {
                       Id = message.Id,
                       TargetFlow = MessageFlow,
                       Operations = Convert(plainChanges),
                   };
        }

        private IEnumerable<FactOperation> Convert(IEnumerable<ErmOperation> changes)
        {
            foreach (var change in changes)
            {
                if (change.EntityType is UnknownEntityType)
                {
                    continue;
                }

                // COMMENT {a.rechkalov, 05.05.2015}: Тут происходит неявное и пока автоматическое преобразование Erm типа (представленного IEntityType) в тип из контекста фактов
                // Возможно, позднее потребуется выделить это преобразование в явный шаг.
                var entityType = EntityTypeMap<FactsContext>.AsEntityType(change.EntityType);
                switch (change.Change)
                {
                    case ChangesType.Added:
                        yield return new CreateFact(entityType, change.EntityId);
                        break;
                    case ChangesType.Updated:
                        yield return new UpdateFact(entityType, change.EntityId);
                        break;
                    case ChangesType.Deleted:
                        yield return new DeleteFact(entityType, change.EntityId);
                        break;
                }
            }
        }

        private class ErmOperation
        {
            public ErmOperation(IEntityType entityType, long entityId, ChangesType change)
            {
                EntityType = entityType;
                EntityId = entityId;
                Change = change;
            }

            public IEntityType EntityType { get; private set; }
            public long EntityId { get; private set; }
            public ChangesType Change { get; private set; }
        }
    }
}
