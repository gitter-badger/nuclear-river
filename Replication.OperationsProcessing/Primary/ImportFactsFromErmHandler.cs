﻿using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly ErmFactsTransformation _ermFactsTransformation;
        private readonly SqlStoreSender _sender;
        private readonly ITracer _tracer;
        private readonly IProfiler _profiler;

        public ImportFactsFromErmHandler(ErmFactsTransformation ermFactsTransformation, SqlStoreSender sender, ITracer tracer, IProfiler profiler)
        {
            _ermFactsTransformation = ermFactsTransformation;
            _sender = sender;
            _tracer = tracer;
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
                var message = messages.OfType<FactOperationAggregatableMessage>().Single();
                var aggregateOperations = _ermFactsTransformation.Transform(message.Operations);
                _profiler.Report<ErmFactOperationProcessedCountIdentity>(message.Operations.Count());
                _sender.Push(aggregateOperations, AggregatesFlow.Instance);

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
