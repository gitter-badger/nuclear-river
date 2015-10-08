using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromErmHandler : IMessageProcessingHandler
    {
        private readonly PrimaryStageCompositeTransformation _transformation;
        private readonly SqlStoreSender _sender;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromErmHandler(PrimaryStageCompositeTransformation transformation, SqlStoreSender sender, ITracer tracer, ITelemetryPublisher telemetryPublisher)
        {
            _sender = sender;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _transformation = transformation;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                var operations = processingResultsMap.SelectMany(pair => pair.Value)
                                                  .Cast<OperationAggregatableMessage<FactOperation>>()
                                                  .SelectMany(message => message.Operations)
                                                  .ToArray();
                Handle(operations);

                var operationPerformTime = processingResultsMap.SelectMany(pair => pair.Value)
                                            .Select(message => message)
                                            .OfType<OperationAggregatableMessage<FactOperation>>()
                                            .Max(message => message.OperationTime);

                _telemetryPublisher.Publish<PrimaryProcessingDelayIdentity>((long)(DateTime.UtcNow - operationPerformTime).TotalMilliseconds);

                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error then import facts for ERM");
                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex));
            }
        }

        private void Handle(IReadOnlyCollection<FactOperation> operations)
        {
            var result = _transformation.Transform(operations);
            _telemetryPublisher.Publish<ErmProcessedOperationCountIdentity>(operations.Count);

            var statisticOperations = result.OfType<CalculateStatisticsOperation>().ToList();
            _sender.Push(statisticOperations, StatisticsFlow.Instance);
            _telemetryPublisher.Publish<StatisticsEnqueuedOperationCountIdentity>(statisticOperations.Count());

            var aggregateOperations = result.OfType<AggregateOperation>().ToList();
            _sender.Push(aggregateOperations, AggregatesFlow.Instance);
            _telemetryPublisher.Publish<AggregateEnqueuedOperationCountIdentity>(aggregateOperations.Count());
        }
    }
}
