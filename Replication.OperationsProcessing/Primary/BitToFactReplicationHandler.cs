using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class BitToFactReplicationHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly BitFactsTransformation _bitFactsTransformation;
        private readonly SqlStoreSender _sender;

        public BitToFactReplicationHandler(BitFactsTransformation bitFactsTransformation, SqlStoreSender sender)
        {
            _bitFactsTransformation = bitFactsTransformation;
            _sender = sender;
        }

        public IEnumerable<StageResult> Handle(IEnumerable<KeyValuePair<Guid, List<IAggregatableMessage>>> processingResultBuckets)
        {
            return processingResultBuckets.Select(pair => Handle(pair.Key, pair.Value)).ToArray();
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                foreach (var message in messages)
                {
                    object dto = null; // todo
                    if (dto is FirmStatisticsDto)
                    {
                        var aggregateOperations = _bitFactsTransformation.Transform((FirmStatisticsDto)dto);
                        _sender.Push(aggregateOperations, message.TargetFlow);
                    }

                    if (dto is CategoryStatisticsDto)
                    {
                        var aggregateOperations = _bitFactsTransformation.Transform((CategoryStatisticsDto)dto);
                        _sender.Push(aggregateOperations, message.TargetFlow);
                    }
                }

                return MessageProcessingStage.Handle.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                return MessageProcessingStage.Handle.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}
