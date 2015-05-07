using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class ErmToFactReplicationHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly FactsTransformation _factsTransformation;
        private readonly SqlStoreSender _sender;

        public ErmToFactReplicationHandler(FactsTransformation factsTransformation, SqlStoreSender sender)
        {
            _factsTransformation = factsTransformation;
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
                var message = messages.OfType<FactOperationAggregatableMessage>().Single();
                var aggregateOperations = _factsTransformation.Transform(message.Operations);

                _sender.Push(aggregateOperations, message.TargetFlow);

                return MessageProcessingStage.Handle.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                return MessageProcessingStage.Handle.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}
