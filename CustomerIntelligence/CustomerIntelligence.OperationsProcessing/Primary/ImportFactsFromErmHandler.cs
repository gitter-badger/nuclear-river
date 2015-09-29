using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.CustomerIntelligence.Domain.Model.Facts;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Metadata.Operations;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromErmHandler : IMessageProcessingHandler
    {
        private readonly IFactsReplicator _factsReplicator;
        private readonly SqlStoreSender _sender;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromErmHandler(IFactsReplicator factsReplicator, SqlStoreSender sender, ITracer tracer, ITelemetryPublisher telemetryPublisher)
        {
            _sender = sender;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _factsReplicator = factsReplicator;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value));
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                _tracer.Debug("Handing fact operations started");
                foreach (var message in messages.OfType<OperationAggregatableMessage<FactOperation>>().ToArray())
                {
                    _tracer.DebugFormat("Replicating operations from use case '{0}'", message.Id);
                    var result = _factsReplicator.Replicate(message.Operations, new FactTypePriorityComparer());
                    _tracer.DebugFormat("Operations from use case '{0}' successfully replicated", message.Id);

                    _telemetryPublisher.Publish<ErmProcessedOperationCountIdentity>(message.Operations.Count);

                    var statistics = result.OfType<RecalculateStatisticsOperation>().ToArray();
                    var aggregates = result.OfType<AggregateOperation>().ToArray();

                    // We always need to use different transaction scope to operate with operation sender because it has its own store
                    using (var pushTransaction = new TransactionScope(TransactionScopeOption.RequiresNew,
                                                                      new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                    {
                        _tracer.Debug("Pushing events for statistics recalculation");
                        _sender.Push(statistics, StatisticsFlow.Instance);
                        _telemetryPublisher.Publish<StatisticsEnqueuedOperationCountIdentity>(statistics.Length);

                        _tracer.Debug("Pushing events for aggregates recalculation");
                        _sender.Push(aggregates, AggregatesFlow.Instance);
                        _telemetryPublisher.Publish<AggregateEnqueuedOperationCountIdentity>(aggregates.Length);

                        pushTransaction.Complete();
                    }

                    _telemetryPublisher.Publish<PrimaryProcessingDelayIdentity>((long)(DateTime.UtcNow - message.OperationTime).TotalMilliseconds);
                }

                _tracer.Debug("Handing fact operations finished");

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
