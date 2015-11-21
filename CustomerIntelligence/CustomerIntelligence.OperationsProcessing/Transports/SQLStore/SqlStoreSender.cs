using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.Storage.API.Writings;
using NuClear.Telemetry.Probing;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender
    {
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IRepository<PerformedOperationFinalProcessing> _repository;
        private readonly AggregateOperationSerializer _aggregateOperationSerializer;

        public SqlStoreSender(
            IIdentityGenerator identityGenerator,
            IRepository<PerformedOperationFinalProcessing> repository,
            AggregateOperationSerializer aggregateOperationSerializer)
        {
            _identityGenerator = identityGenerator;
            _repository = repository;
            _aggregateOperationSerializer = aggregateOperationSerializer;
        }

        public void Push(IEnumerable<RecalculateStatisticsOperation> operations, IMessageFlow targetFlow)
        {
            using (Probe.Create("Send Statistics Operations"))
            {
                var transportMessages = operations.Select(operation => SerializeMessage(operation, targetFlow));
                Save(transportMessages.ToArray());
            }
        }

        public void Push(IEnumerable<AggregateOperation> operations, IMessageFlow targetFlow)
        {
            using (Probe.Create("Send Aggregate Operations"))
            {
                var transportMessages = operations.Select(operation => _aggregateOperationSerializer.Serialize(operation, targetFlow));
                Save(transportMessages.ToArray());
            }
        }

        private void Save(IReadOnlyCollection<PerformedOperationFinalProcessing> transportMessages)
        {
            foreach (var transportMessage in transportMessages)
            {
                transportMessage.Id = _identityGenerator.Next();
            }

            _repository.AddRange(transportMessages);
            _repository.Save();
        }

        private static PerformedOperationFinalProcessing SerializeMessage(RecalculateStatisticsOperation operation, IMessageFlow targetFlow)
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
