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
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value)).ToArray();
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                var operations = messages.OfType<FactOperationAggregatableMessage>().Single().Operations;

                var result = _transformation.Transform(operations);
                _telemetryPublisher.Publish<ErmProcessedOperationCountIdentity>(operations.Count());

                var statisticOperations = result.OfType<CalculateStatisticsOperation>().ToList();
                _sender.Push(statisticOperations, StatisticsFlow.Instance);
                _telemetryPublisher.Publish<StatisticsEnquiedOperationCountIdentity>(statisticOperations.Count());

                var aggregateOperations = result.OfType<AggregateOperation>().ToList();
                _sender.Push(aggregateOperations, AggregatesFlow.Instance);
                _telemetryPublisher.Publish<AggregateEnquiedOperationCountIdentity>(aggregateOperations.Count());

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error then import facts for ERM");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}
