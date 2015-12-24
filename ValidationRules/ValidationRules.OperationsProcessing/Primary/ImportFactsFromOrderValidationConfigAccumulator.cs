using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Transports.File;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Domain.Dto;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.OperationsProcessing.Identities.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromOrderValidationConfigAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromOrderValidationConfigFlow, FileContentMessage, CorporateBusAggregatableMessage>
    {
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly OrderValidationConfigParser _parser;

        public ImportFactsFromOrderValidationConfigAccumulator(ITracer tracer, ITelemetryPublisher telemetryPublisher, OrderValidationConfigParser parser)
        {
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _parser = parser;
        }

        protected override CorporateBusAggregatableMessage Process(FileContentMessage message)
        {
            _tracer.Debug("Config parsing");
            _telemetryPublisher.Publish<ConfigUpdateCountIdentity>(1);
            return new CorporateBusAggregatableMessage
                {
                    TargetFlow = MessageFlow,
                    Dtos = new[] { _parser.Parse(message.ContentProvider.Invoke()) }
                };
        }
    }
}
