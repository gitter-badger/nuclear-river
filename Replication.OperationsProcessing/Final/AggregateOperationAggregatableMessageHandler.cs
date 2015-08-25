using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public sealed class AggregateOperationAggregatableMessageHandler : IMessageProcessingHandler
    {
        private readonly CustomerIntelligenceTransformation _customerIntelligenceTransformation;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public AggregateOperationAggregatableMessageHandler(CustomerIntelligenceTransformation customerIntelligenceTransformation, ITracer tracer, ITelemetryPublisher telemetryPublisher)
        {
            _customerIntelligenceTransformation = customerIntelligenceTransformation;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value));
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                foreach (var message in messages.OfType<OperationAggregatableMessage<AggregateOperation>>())
                {
                    _customerIntelligenceTransformation.Transform(message.Operations);
                    _telemetryPublisher.Publish<AggregateProcessedOperationCountIdentity>(message.Operations.Count);

                    _telemetryPublisher.Publish<AggregateProcessingDelayIdentity>((long)(DateTime.UtcNow - message.OperationTime).TotalMilliseconds);
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error then calculating aggregates");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}