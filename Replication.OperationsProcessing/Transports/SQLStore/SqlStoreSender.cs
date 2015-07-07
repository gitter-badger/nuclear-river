using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using LinqToDB;
using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Metadata.Model.Context;
using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender
    {
        private readonly DataConnection _dataConnection;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public SqlStoreSender(IDataContext dataConnection, ITelemetryPublisher telemetryPublisher)
        {
            _dataConnection = (DataConnection)dataConnection;
            _telemetryPublisher = telemetryPublisher;
        }

        public void Push(IEnumerable<AggregateOperation> operations, IMessageFlow targetFlow)
        {
            _telemetryPublisher.Trace("Sending");

            var transportMessages = operations.Select(operation => SerializeMessage(operation, targetFlow));
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
