using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromErmHandler : IMessageProcessingHandler
    {
        private readonly ErmFactsTransformation _transformation;
        private readonly SqlStoreSender _sender;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromErmHandler(ErmFactsTransformation transformation, SqlStoreSender sender, ITracer tracer, ITelemetryPublisher telemetryPublisher)
        {
            _sender = sender;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _transformation = transformation;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value));
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                var statisticsOperations = new List<CalculateStatisticsOperation>();
                var aggregateOperations = new List<AggregateOperation>();
                var minOperationTime = DateTime.UtcNow;

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                {
                    foreach (var message in messages.OfType<OperationAggregatableMessage<FactOperation>>().ToArray())
                    {
                        var result = _transformation.Transform(message.Operations);
                        _telemetryPublisher.Publish<ErmProcessedOperationCountIdentity>(message.Operations.Count);

                        var statistics = result.OfType<CalculateStatisticsOperation>().ToArray();
                        statisticsOperations.AddRange(statistics);

                        var aggregates = result.OfType<AggregateOperation>().ToArray();
                        aggregateOperations.AddRange(aggregates);

                        if (message.OperationTime < minOperationTime)
                        {
                            minOperationTime = message.OperationTime;
                        }
                    }

                    // We need to use different transaction scope to operate with operation sender because it has its own store
                    using (var pushTransaction = new TransactionScope(TransactionScopeOption.RequiresNew,
                                                                      new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                    {
                        _sender.Push(statisticsOperations, StatisticsFlow.Instance);
                        _telemetryPublisher.Publish<StatisticsEnqueuedOperationCountIdentity>(statisticsOperations.Count);

                        _sender.Push(aggregateOperations, AggregatesFlow.Instance);
                        _telemetryPublisher.Publish<AggregateEnqueuedOperationCountIdentity>(aggregateOperations.Count);

                        pushTransaction.Complete();
                    }

                    _telemetryPublisher.Publish<PrimaryProcessingDelayIdentity>((long)(DateTime.UtcNow - minOperationTime).TotalMilliseconds);

                    transaction.Complete();
                }
                
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error then import facts for ERM");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}
