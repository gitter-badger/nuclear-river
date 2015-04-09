using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Messaging.Metadata.Flows;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;

namespace NuClear.AdvancedSearch.Messaging
{
    public sealed class ReplicateToAdvancedSearchMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        public IEnumerable<StageResult> Handle(IEnumerable<KeyValuePair<Guid, List<IAggregatableMessage>>> processingResultBuckets)
        {
            var results = processingResultBuckets.Select(x => new
            {
                OriginalMessageId = x.Key,
                WellKnownMessages = x.Value
                    .Where(y => y.TargetFlow.Equals(Replicate2AdvancedSearchFlow.Instance))
                    .OfType<Replicate2AdvancedSearchAggregatableMessage>()
                    .ToList(),
            })
            .Where(x => x.WellKnownMessages.Any())
            .Select(x => HandleMessage(x.OriginalMessageId, x.WellKnownMessages));

            return results;
        }

        private static StageResult HandleMessage(Guid originalMessageId, IEnumerable<Replicate2AdvancedSearchAggregatableMessage> messages)
        {
            try
            {
                return MessageProcessingStage.Handle.ResultFor(originalMessageId).AsSucceeded();
            }
            catch (Exception ex)
            {
                return MessageProcessingStage.Handle.ResultFor(originalMessageId).WithExceptions(ex).AsFailed();
            }
        }
    }
}