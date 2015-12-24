using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.ValidationRules.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromOrderValidationConfigHandler : IMessageProcessingHandler
    {
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly IStatisticsImporterFactory _factory;

        public ImportFactsFromOrderValidationConfigHandler(
            IStatisticsImporterFactory factory,
            ITracer tracer,
            ITelemetryPublisher telemetryPublisher)
        {
            _factory = factory;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(bucket => HandleBucket(bucket.Key, bucket.Value)).ToArray();
        }

        private StageResult HandleBucket(Guid id, IEnumerable<IAggregatableMessage> bucket)
        {
            try
            {
                foreach (var message in bucket.Cast<CorporateBusAggregatableMessage>())
                {
                    foreach (var dto in message.Dtos)
                    {
                        foreach (var processor in _factory.Create(dto.GetType()))
                        {
                            processor.Import(dto);
                        }
                    }
                }

                return MessageProcessingStage.Handling.ResultFor(id).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when import order validation config");
                return MessageProcessingStage.Handling.ResultFor(id).AsFailed().WithExceptions(ex);
            }
        }
    }
}
