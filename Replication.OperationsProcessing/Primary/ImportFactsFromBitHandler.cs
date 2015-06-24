using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitHandler : IMessageProcessingHandler
    {
        private readonly BitFactsTransformation _bitFactsTransformation;
        private readonly SqlStoreSender _sender;
        private readonly ITransactionsManager _transactionsManager;
        private readonly ITracer _tracer;
        private readonly IProfiler _profiler;

        public ImportFactsFromBitHandler(BitFactsTransformation bitFactsTransformation, SqlStoreSender sender, ITransactionsManager transactionsManager, ITracer tracer, IProfiler profiler)
        {
            _bitFactsTransformation = bitFactsTransformation;
            _sender = sender;
            _transactionsManager = transactionsManager;
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
                foreach (var message in messages.OfType<CorporateBusDtoMessage>())
                {
                    foreach (var dto in message.Dtos)
                    {
                        try
                        {
                            // Не весь пакет в одной транзакции, ибо по каждому объекту очень много изменений.
                            _transactionsManager.BeginTransaction();
                            
                            var firmStatisticsDto = dto as FirmStatisticsDto;
                            if (firmStatisticsDto != null)
                            {
                                var aggregateOperations = _bitFactsTransformation.Transform(firmStatisticsDto);
                                _profiler.Report<BitStatisticsEntityProcessedCountIdentity>(firmStatisticsDto.Firms.Count());
                                _sender.Push(aggregateOperations, AggregatesFlow.Instance);
                            }

                            var categoryStatisticsDto = dto as CategoryStatisticsDto;
                            if (categoryStatisticsDto != null)
                            {
                                var aggregateOperations = _bitFactsTransformation.Transform(categoryStatisticsDto);
                                _profiler.Report<BitStatisticsEntityProcessedCountIdentity>(categoryStatisticsDto.Categories.Count());
                                _sender.Push(aggregateOperations, AggregatesFlow.Instance);
                            }

                            _transactionsManager.CommitTransaction();
                        }
                        catch (Exception)
                        {
                            _transactionsManager.RollbackTransaction();
                            throw;
                        }
                    }
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error then import facts for BIT");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}
