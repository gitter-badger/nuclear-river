using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Metadata.Model.Context;
using NuClear.Replication.OperationsProcessing.Metadata.Operations;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public class AggregateOperationSerializer
    {
	    private readonly IEntityTypeMappingRegistry<CustomerIntelligenceContext> _registry;

	    private static readonly Dictionary<Guid, Type> OperationIdRegistry =
            new Dictionary<Guid, Type>
            {
                { InitializeAggregateOperationIdentity.Instance.Guid, typeof(InitializeAggregate) },
                { RecalculateAggregateOperationIdentity.Instance.Guid, typeof(RecalculateAggregate) },
                { DestroyAggregateOperationIdentity.Instance.Guid, typeof(DestroyAggregate) }
            };

        private static readonly Dictionary<Type, Guid> OperationTypeRegistry =
            OperationIdRegistry.ToDictionary(x => x.Value, x => x.Key);

	    public AggregateOperationSerializer(IEntityTypeMappingRegistry<CustomerIntelligenceContext> registry)
	    {
		    _registry = registry;
	    }

		public AggregateOperation Deserialize(PerformedOperationFinalProcessing operation)
		{
			Type operationType;
			if (OperationIdRegistry.TryGetValue(operation.OperationId, out operationType))
			{
				var entityName = EntityType.Instance.Parse(operation.EntityTypeId);
				return (AggregateOperation)Activator.CreateInstance(operationType, _registry.GetEntityType(entityName), operation.EntityId);
			}

			throw new ArgumentException($"Unknown operation id {operation.OperationId}", nameof(operation));
		}

		public PerformedOperationFinalProcessing Serialize(AggregateOperation operation, IMessageFlow targetFlow)
	    {
			Guid guid;
			if (OperationTypeRegistry.TryGetValue(operation.GetType(), out guid))
			{
				var entityType = _registry.GetEntityName(operation.AggregateType);
				return new PerformedOperationFinalProcessing
				{
					CreatedOn = DateTime.UtcNow,
					MessageFlowId = targetFlow.Id,
					EntityId = operation.AggregateId,
					EntityTypeId = entityType.Id,
					OperationId = guid,
				};
			}

			throw new ArgumentException($"Unknown operation type {operation.GetType().Name}", nameof(operation));
		}
    }
}
