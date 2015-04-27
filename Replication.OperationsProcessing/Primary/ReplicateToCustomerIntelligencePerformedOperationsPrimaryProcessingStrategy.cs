using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.OperationsProcessing.Transports.SQLStore.Primary;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class ReplicateToCustomerIntelligencePerformedOperationsPrimaryProcessingStrategy :
        MessageProcessingStrategyBase<Replicate2CustomerIntelligenceFlow, TrackedUseCase, PrimaryAggregatableMessage>
    {
        private readonly ITracer _tracer;

        public ReplicateToCustomerIntelligencePerformedOperationsPrimaryProcessingStrategy(ITracer tracer)
        {
            _tracer = tracer;
        }

        protected override PrimaryAggregatableMessage Process(TrackedUseCase message)
        {
            _tracer.InfoFormat("Get tracked use case from service bus: {0}", message.ToString());

            var results = message.Operations.SelectMany(x => x.ChangesContext.UntypedChanges)
                   .GroupBy(x => x.Key, x => x.Value.Select(y => new PerformedOperationFinalProcessing
                                                     {
                                                         MessageFlowId = MessageFlow.Id,
                                                         EntityId = y.Key,
                                                         EntityTypeId = x.Key.Id,
                                                         OperationId = message.Id
                                                     }))
                                                     .SelectMany(x => x.SelectMany(y => y));

            return new PrimaryAggregatableMessage
            {
                TargetFlow = MessageFlow,
                Results = results,
            };
        }
    }
}