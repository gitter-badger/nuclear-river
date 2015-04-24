using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Transport;

namespace NuClear.Replication.OperationsProcessing.Stages
{
    public sealed class FactProcessingHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly InternalSender _sender;

        public FactProcessingHandler(InternalSender sender)
        {
            _sender = sender;
        }

        public IEnumerable<StageResult> Handle(IEnumerable<KeyValuePair<Guid, List<IAggregatableMessage>>> processingResultBuckets)
        {
            // TODO {a.rechkalov, 23.04.2015}: Вызвать обновление фактов и получить Operations
            _sender.Push(new[] { new AggregateOperation(), new AggregateOperation(), });
            return processingResultBuckets.Select(pair => MessageProcessingStage.Handle.ResultFor(pair.Key).AsSucceeded());
        }
    }
}
