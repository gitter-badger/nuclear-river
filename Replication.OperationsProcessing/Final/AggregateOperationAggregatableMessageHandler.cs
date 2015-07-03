using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public class AggregateOperationAggregatableMessageHandler : IMessageProcessingHandler
    {
        private readonly CustomerIntelligenceTransformation _customerIntelligenceTransformation;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public AggregateOperationAggregatableMessageHandler(CustomerIntelligenceTransformation customerIntelligenceTransformation, ITelemetryPublisher telemetryPublisher)
        {
            _customerIntelligenceTransformation = customerIntelligenceTransformation;
            _telemetryPublisher = telemetryPublisher;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value)).ToArray();
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                var message = messages.OfType<AggregateOperationAggregatableMessage>().Single();
                _customerIntelligenceTransformation.Transform(message.Operations);
                _telemetryPublisher.Publish<AggregateOperationProcessedCountIdentity>(message.Operations.Count());

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}