using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitHandler : IMessageProcessingHandler
    {
        private readonly IStatisticsImporterFactory _statisticsImporterFactory;
        private readonly SqlStoreSender _sender;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromBitHandler(IStatisticsImporterFactory statisticsImporterFactory, SqlStoreSender sender, ITracer tracer, ITelemetryPublisher telemetryPublisher)
        {
            _statisticsImporterFactory = statisticsImporterFactory;
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
                            var firmStatisticsImporter = _statisticsImporterFactory.Create(typeof(FirmStatisticsDto));

                            var calculateStatisticsOperations = firmStatisticsImporter.Import(firmStatisticsDto);
                            _telemetryPublisher.Publish<BitStatisticsEntityProcessedCountIdentity>(firmStatisticsDto.Firms.Count);
                            _sender.Push(calculateStatisticsOperations, StatisticsFlow.Instance);
                        }

                        var categoryStatisticsDto = dto as CategoryStatisticsDto;
                        if (categoryStatisticsDto != null)
                        {
                            var categoryStatisticsImporter = _statisticsImporterFactory.Create(typeof(CategoryStatisticsDto));

                            var calculateStatisticsOperations = categoryStatisticsImporter.Import(categoryStatisticsDto);
                            _telemetryPublisher.Publish<BitStatisticsEntityProcessedCountIdentity>(categoryStatisticsDto.Categories.Count);
                            _sender.Push(calculateStatisticsOperations, StatisticsFlow.Instance);
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
