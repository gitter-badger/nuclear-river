using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Metadata.Model.Context;
using NuClear.Storage.Writings;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender
    {
        private readonly IRepository<PerformedOperationFinalProcessing> _repository;

        public SqlStoreSender(IRepository<PerformedOperationFinalProcessing> repository)
        {
            _repository = repository;
        }

        public void Push(IEnumerable<CalculateStatisticsOperation> operations, IMessageFlow targetFlow)
        {
            using (Probe.Create("Send Statistics Operations"))
            {
                var transportMessages = operations.Select(operation => SerializeMessage(operation, targetFlow));
                Save(transportMessages);
            }
        }

        public void Push(IEnumerable<AggregateOperation> operations, IMessageFlow targetFlow)
        {
            using (Probe.Create("Send Aggregate Operations"))
            {
                var transportMessages = operations.Select(operation => SerializeMessage(operation, targetFlow));
                Save(transportMessages);
            }
        }

        private void Save(IEnumerable<PerformedOperationFinalProcessing> transportMessages)
        {
            _repository.AddRange(transportMessages);
            _repository.Save();
        }

        private static PerformedOperationFinalProcessing SerializeMessage(CalculateStatisticsOperation operation, IMessageFlow targetFlow)
        {
            var now = DateTime.UtcNow;
            return new PerformedOperationFinalProcessing
                   {
                       Id = now.Ticks, // PerformedOperationFinalProcessing type has incorrect implementation if Equals and GetHashCode
                       CreatedOn = now,
                       MessageFlowId = targetFlow.Id,
                       Context = operation.Serialize().ToString(),
                       OperationId = operation.GetIdentity(),
                   };
        }

        private static PerformedOperationFinalProcessing SerializeMessage(AggregateOperation operation, IMessageFlow targetFlow)
        {
            var entityType = EntityTypeMap<CustomerIntelligenceContext>.AsEntityName(operation.AggregateType);
            var now = DateTime.UtcNow;
            return new PerformedOperationFinalProcessing
                   {
                       Id = now.Ticks, // PerformedOperationFinalProcessing type has incorrect implementation if Equals and GetHashCode
                       CreatedOn = now,
                       MessageFlowId = targetFlow.Id,
                       EntityId = operation.AggregateId,
                       EntityTypeId = entityType.Id,
                       OperationId = operation.GetIdentity(),
                   };
        }
    }
}
