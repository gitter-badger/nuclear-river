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

namespace NuClear.Replication.OperationsProcessing.Final
{
    public sealed class StatisticsAggregatableMessageHandler : IMessageProcessingHandler
    {
        private readonly StatisticsFinalTransformation _statisticsTransformation;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public StatisticsAggregatableMessageHandler(StatisticsFinalTransformation statisticsTransformation, ITelemetryPublisher telemetryPublisher)
        {
            _statisticsTransformation = statisticsTransformation;
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
                foreach (var message in messages.OfType<OperationAggregatableMessage<CalculateStatisticsOperation>>())
                {
                    _statisticsTransformation.Recalculate(message.Operations);
                    _telemetryPublisher.Publish<StatisticsProcessedOperationCountIdentity>(message.Operations.Count);

                    _telemetryPublisher.Publish<StatisticsProcessingDelayIdentity>((long)(DateTime.UtcNow - message.OperationTime).TotalMilliseconds);
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}