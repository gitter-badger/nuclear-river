using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;

namespace NuClear.Replication.OperationsProcessing.Stages
{
    public sealed class CustomerIntelligenceProcessingHandler : IMessageAggregatedProcessingResultsHandler
    {
        public IEnumerable<StageResult> Handle(IEnumerable<KeyValuePair<Guid, List<IAggregatableMessage>>> processingResultBuckets)
        {
            throw new NotImplementedException();
        }
    }
}
