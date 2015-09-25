using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using LinqToDB;
using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender
    {
	    private readonly AggregateOperationSerializer _aggregateOperationSerializer;
	    private readonly DataConnection _dataConnection;

        public SqlStoreSender(IDataContext dataConnection, AggregateOperationSerializer aggregateOperationSerializer)
        {
	        _aggregateOperationSerializer = aggregateOperationSerializer;
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
                var transportMessages = operations.Select(operation => _aggregateOperationSerializer.Serialize(operation, targetFlow));
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
    }
}
