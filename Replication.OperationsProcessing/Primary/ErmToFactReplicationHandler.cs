using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Replication.OperationsProcessing.Transports.InProc;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class ErmToFactReplicationHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly FactsTransformation _factsTransformation;

        public ErmToFactReplicationHandler(FactsTransformation factsTransformation)
        {
            _factsTransformation = factsTransformation;
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
                
                return MessageProcessingStage.Handle.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                return MessageProcessingStage.Handle.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}
