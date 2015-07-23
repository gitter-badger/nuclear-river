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
        private readonly ErmFactsTransformation _ermFactsTransformation;
        private readonly StatisticsPrimaryTransformation _statisticsTransformation;
        private readonly SqlStoreSender _sender;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromErmHandler(ErmFactsTransformation ermFactsTransformation, SqlStoreSender sender, ITracer tracer, ITelemetryPublisher telemetryPublisher, StatisticsPrimaryTransformation statisticsTransformation)
        {
            _ermFactsTransformation = ermFactsTransformation;
            _sender = sender;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _statisticsTransformation = statisticsTransformation;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value)).ToArray();
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                IReadOnlyCollection<FactOperation> operations = messages.OfType<FactOperationAggregatableMessage>().Single().Operations;

                var statisticsOperations = _statisticsTransformation.DetectStatisticsOperations(operations);

                var aggregateOperations = _ermFactsTransformation.Transform(operations).Distinct().ToList();
                _telemetryPublisher.Publish<ErmProcessedOperationCountIdentity>(operations.Count());

                statisticsOperations = statisticsOperations.Concat(_statisticsTransformation.DetectStatisticsOperations(operations)).Distinct().ToList();

                _sender.Push(statisticsOperations, StatisticsFlow.Instance);
                _telemetryPublisher.Publish<StatisticsEnquiedOperationCountIdentity>(aggregateOperations.Count());

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
