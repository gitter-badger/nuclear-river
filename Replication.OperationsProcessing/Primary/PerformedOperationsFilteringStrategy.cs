using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
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
        private readonly ErmOperationAdapter _adapter;

        public PerformedOperationsFilteringStrategy(ITracer tracer)
        {
            _tracer = tracer;
            _adapter = new ErmOperationAdapter();
        }

        protected override FactOperationAggregatableMessage Process(TrackedUseCase message)
        {
            _tracer.DebugFormat("Processing TUC {0}", message.Id);

            var plainChanges =
                message.Operations.SelectMany(scope => scope.ChangesContext.UntypedChanges)
                       .SelectMany(
                           x => x.Value.SelectMany(
                               y => y.Value.Details.Select(
                                   z => new ErmOperationAdapter.ErmOperation(x.Key, y.Key, z.ChangesType))));

            return new FactOperationAggregatableMessage
                   {
                       Id = message.Id,
                       TargetFlow = MessageFlow,
                       Operations = _adapter.Convert(plainChanges),
                   };
        }
    }
}
