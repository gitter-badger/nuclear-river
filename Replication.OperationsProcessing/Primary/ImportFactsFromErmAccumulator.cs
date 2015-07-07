using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Model.Common.Entities;
using NuClear.OperationsTracking.API.Changes;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Metadata.Model.Context;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    /// <summary>
    /// Стратегия выполняет фильтрацию операций, приехавших в TUC, и преобразование этих операций в операции над фактами.
    /// </summary>
    public sealed class ImportFactsFromErmAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromErmFlow, TrackedUseCase, FactOperationAggregatableMessage>
    {
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromErmAccumulator(ITracer tracer, ITelemetryPublisher telemetryPublisher)
        {
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
        }

        protected override FactOperationAggregatableMessage Process(TrackedUseCase message)
        {
            _tracer.DebugFormat("Processing TUC {0}", message.Id);

            var plainChanges =
                message.Operations.SelectMany(scope => scope.AffectedEntities.Changes)
                       .SelectMany(
                           x => x.Value.SelectMany(
                               y => y.Value.Select(
                                   z => new ErmOperation(x.Key, y.Key, z.ChangeKind))))
                       .ToList();

            _telemetryPublisher.Publish<ErmOperationCountIdentity>(plainChanges.Count);

            return new FactOperationAggregatableMessage
                   {
                       Id = message.Id,
                       TargetFlow = MessageFlow,
                       Operations = Convert(plainChanges),
                   };
        }

        private IEnumerable<FactOperation> Convert(IEnumerable<ErmOperation> operations)
        {
            return from operation in operations
                   where !(operation.EntityType is UnknownEntityType)
                   let entityType = EntityTypeMap<FactsContext>.AsEntityType(operation.EntityType)
                   select new FactOperation(entityType, operation.EntityId);
        }

        private class ErmOperation
        {
            public ErmOperation(IEntityType entityType, long entityId, ChangeKind change)
            {
                EntityType = entityType;
                EntityId = entityId;
                Change = change;
            }

            public IEntityType EntityType { get; private set; }
            public long EntityId { get; private set; }
            public ChangeKind Change { get; private set; }
        }
    }
}
