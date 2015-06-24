using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public class AggregateOperationAggregatableMessageHandler : IMessageProcessingHandler
    {
        private readonly CustomerIntelligenceTransformation _customerIntelligenceTransformation;
        private readonly ITransactionsManager _transactionsManager;
        private readonly IProfiler _profiler;

        public AggregateOperationAggregatableMessageHandler(CustomerIntelligenceTransformation customerIntelligenceTransformation, ITransactionsManager transactionsManager, IProfiler profiler)
        {
            _customerIntelligenceTransformation = customerIntelligenceTransformation;
            _transactionsManager = transactionsManager;
            _profiler = profiler;
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

                _transactionsManager.BeginTransaction();
                _customerIntelligenceTransformation.Transform(message.Operations);
                _profiler.Report<AggregateOperationProcessedCountIdentity>(message.Operations.Count());
                _transactionsManager.CommitTransaction();

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _transactionsManager.RollbackTransaction();
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}