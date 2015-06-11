﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitHandler : IMessageProcessingHandler
    {
        private readonly BitFactsTransformation _bitFactsTransformation;
        private readonly SqlStoreSender _sender;

        public ImportFactsFromBitHandler(BitFactsTransformation bitFactsTransformation, SqlStoreSender sender)
        {
            _bitFactsTransformation = bitFactsTransformation;
            _sender = sender;
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
                        var firmStatisticsDto = dto as FirmStatisticsDto;
                        if (firmStatisticsDto != null)
                    {
                        var aggregateOperations = _bitFactsTransformation.Transform(firmStatisticsDto);
                        _sender.Push(aggregateOperations, AggregatesFlow.Instance);
                    }

                        var categoryStatisticsDto = dto as CategoryStatisticsDto;
                        if (categoryStatisticsDto != null)
                    {
                        var aggregateOperations = _bitFactsTransformation.Transform(categoryStatisticsDto);
                        _sender.Push(aggregateOperations, AggregatesFlow.Instance);
                    }
                }
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}
