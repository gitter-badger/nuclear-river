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

namespace NuClear.Replication.OperationsProcessing.Stages
{
    public sealed class FactProcessingHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly FactsTransformation _factsTransformation;
        private readonly InProcBridgeSender _sender;

        public FactProcessingHandler(InProcBridgeSender sender, FactsTransformation factsTransformation)
        {
            _sender = sender;
            _factsTransformation = factsTransformation;
        }

        public IEnumerable<StageResult> Handle(IEnumerable<KeyValuePair<Guid, List<IAggregatableMessage>>> processingResultBuckets)
        {
            return processingResultBuckets.Select(pair => Handle(pair.Key, pair.Value)).ToArray();
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            var operations = new List<AggregateOperation>();
            foreach (var message in messages.OfType<FactAggregatableMessage>())
            {
                operations.AddRange(_factsTransformation.Transform(message.Operations));
            }

            _sender.Push(new ReplicationMessage<AggregateOperation>
                         {
                             Id = Guid.NewGuid(),
                             Operations = operations,
                         });
            
            return MessageProcessingStage.Handle.ResultFor(bucketId).AsSucceeded();
        }
    }
}
