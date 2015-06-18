using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public sealed class AggregateOperationAggregatableMessageHandler : IMessageProcessingHandler
    {
        private readonly CustomerIntelligenceTransformation _customerIntelligenceTransformation;
        private readonly ITracer _tracer;

        public AggregateOperationAggregatableMessageHandler(CustomerIntelligenceTransformation customerIntelligenceTransformation, ITracer tracer)
        {
            _customerIntelligenceTransformation = customerIntelligenceTransformation;
            _tracer = tracer;
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

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error then calculating aggregates");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}