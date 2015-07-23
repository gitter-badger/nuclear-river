using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public sealed class StatisticsAggregatableMessageHandler : IMessageProcessingHandler
    {
        private readonly StatisticsFinalTransformation _statisticsTransformation;

        public StatisticsAggregatableMessageHandler(StatisticsFinalTransformation statisticsTransformation)
        {
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
                var operations = messages.OfType<StatisticsAggregatableMessage>().Single().Operations;
                _statisticsTransformation.Recalculate(operations);
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}