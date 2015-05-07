using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Primary;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public class ReplicateToCustomerIntelligenceMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly CustomerIntelligenceTransformation _customerIntelligenceTransformation;

        public ReplicateToCustomerIntelligenceMessageAggregatedProcessingResultHandler(CustomerIntelligenceTransformation customerIntelligenceTransformation)
        {
            _customerIntelligenceTransformation = customerIntelligenceTransformation;
        }

        public IEnumerable<StageResult> Handle(IEnumerable<KeyValuePair<Guid, List<IAggregatableMessage>>> processingResultBuckets)
        {
            return processingResultBuckets.Select(pair => Handle(pair.Key, pair.Value)).ToArray();
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                var message = messages.OfType<AggregateOperationAggregatableMessage>().Single();
                _customerIntelligenceTransformation.Transform(message.Operations);
                
                return MessageProcessingStage.Handle.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                return MessageProcessingStage.Handle.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}