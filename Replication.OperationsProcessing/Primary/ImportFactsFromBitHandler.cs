using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
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
    public sealed class ImportFactsFromBitHandler : IMessageProcessingHandler
    {
        private readonly BitFactsTransformation _bitFactsTransformation;
        private readonly SqlStoreSender _sender;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromBitHandler(BitFactsTransformation bitFactsTransformation, SqlStoreSender sender, ITracer tracer, ITelemetryPublisher telemetryPublisher)
        {
            _bitFactsTransformation = bitFactsTransformation;
            _sender = sender;
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
                foreach (var message in messages.OfType<CorporateBusAggregatableMessage>())
                {
                    foreach (var dto in message.Dtos)
                    {
                        var firmStatisticsDto = dto as FirmStatisticsDto;
                        if (firmStatisticsDto != null)
                        {
                            var aggregateOperations = _bitFactsTransformation.Transform(firmStatisticsDto);
                            _telemetryPublisher.Publish<BitStatisticsEntityProcessedCountIdentity>(firmStatisticsDto.Firms.Count());
                            _sender.Push(aggregateOperations, AggregatesFlow.Instance);
                        }

                        var categoryStatisticsDto = dto as CategoryStatisticsDto;
                        if (categoryStatisticsDto != null)
                        {
                            var aggregateOperations = _bitFactsTransformation.Transform(categoryStatisticsDto);
                            _telemetryPublisher.Publish<BitStatisticsEntityProcessedCountIdentity>(categoryStatisticsDto.Categories.Count());
                            _sender.Push(aggregateOperations, AggregatesFlow.Instance);
                        }
                    }
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error then import facts for BIT");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}
