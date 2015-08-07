using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using LinqToDB;
using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Metadata.Model.Context;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender
    {
        private readonly DataConnection _dataConnection;

        public SqlStoreSender(IDataContext dataConnection)
        {
            _dataConnection = (DataConnection)dataConnection;
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
            try
            {
                _dataConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                foreach (var message in transportMessages)
                {
                    _dataConnection.Insert(message);
                }

                _dataConnection.CommitTransaction();
            }
            catch
            {
                _dataConnection.RollbackTransaction();
                throw;
            }
        }

        private static PerformedOperationFinalProcessing SerializeMessage(CalculateStatisticsOperation operation, IMessageFlow targetFlow)
        {
            return new PerformedOperationFinalProcessing
            {
                CreatedOn = DateTime.UtcNow,
                MessageFlowId = targetFlow.Id,
                Context = operation.Serialize().ToString(),
                OperationId = operation.GetIdentity(),
            };
        }

        private static PerformedOperationFinalProcessing SerializeMessage(AggregateOperation operation, IMessageFlow targetFlow)
        {
            var entityType = EntityTypeMap<CustomerIntelligenceContext>.AsEntityName(operation.AggregateType);
            return new PerformedOperationFinalProcessing
                   {
                       CreatedOn = DateTime.UtcNow,
                       MessageFlowId = targetFlow.Id,
                       EntityId = operation.AggregateId,
                       EntityTypeId = entityType.Id,
                       OperationId = operation.GetIdentity(),
                   };
        }
    }
}
