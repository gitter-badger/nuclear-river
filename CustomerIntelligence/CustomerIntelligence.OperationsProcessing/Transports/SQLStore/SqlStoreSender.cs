using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Contexts;
using NuClear.Messaging.API.Flows;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.Metadata.Operations;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.Storage.Writings;
using NuClear.Telemetry.Probing;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender
    {
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IRepository<PerformedOperationFinalProcessing> _repository;

        public SqlStoreSender(IIdentityGenerator identityGenerator, IRepository<PerformedOperationFinalProcessing> repository)
        {
            _identityGenerator = identityGenerator;
            _repository = repository;
        }

        public void Push(IEnumerable<RecalculateStatisticsOperation> operations, IMessageFlow targetFlow)
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

        private PerformedOperationFinalProcessing SerializeMessage(RecalculateStatisticsOperation operation, IMessageFlow targetFlow)
        {
            return new PerformedOperationFinalProcessing
                   {
                       Id = _identityGenerator.Next(),
                       CreatedOn = DateTime.UtcNow,
                       MessageFlowId = targetFlow.Id,
                       Context = operation.Serialize().ToString(),
                       OperationId = operation.GetIdentity(),
                   };
        }

        private PerformedOperationFinalProcessing SerializeMessage(AggregateOperation operation, IMessageFlow targetFlow)
        {
            var entityType = EntityTypeMap<CustomerIntelligenceContext>.AsEntityName(operation.AggregateType);
            return new PerformedOperationFinalProcessing
                   {
                       Id = _identityGenerator.Next(),
                       CreatedOn = DateTime.UtcNow,
                       MessageFlowId = targetFlow.Id,
                       EntityId = operation.AggregateId,
                       EntityTypeId = entityType.Id,
                       OperationId = operation.GetIdentity(),
                   };
        }
    }
}
