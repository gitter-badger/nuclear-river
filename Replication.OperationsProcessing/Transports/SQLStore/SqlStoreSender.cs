using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Metadata.Model.Context;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender
    {
        private readonly IDataContext _context;

        public SqlStoreSender(IDataContext context)
        {
            _context = context;
        }

        public void Push(IEnumerable<AggregateOperation> operations, IMessageFlow targetFlow)
        {
            var transportMessages = operations.Select(operation => SerializeMessage(operation, targetFlow));
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (var message in transportMessages)
                {
                    _context.Insert(message);
                }

                scope.Complete();
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
