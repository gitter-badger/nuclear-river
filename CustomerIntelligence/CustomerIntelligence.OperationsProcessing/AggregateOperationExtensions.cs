using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Contexts;
using NuClear.Model.Common.Entities;
using NuClear.Replication.Metadata.Operations;
using NuClear.Replication.OperationsProcessing.Identities.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing
{
    public static class AggregateOperationExtensions
    {
        private static readonly Dictionary<Guid, Type> OperationIdRegistry =
            new Dictionary<Guid, Type>
            {
                { InitializeAggregateOperationIdentity.Instance.Guid, typeof(InitializeAggregate) },
                { RecalculateAggregateOperationIdentity.Instance.Guid, typeof(RecalculateAggregate) },
                { DestroyAggregateOperationIdentity.Instance.Guid, typeof(DestroyAggregate) }
            };

        private static readonly Dictionary<Type, Guid> OperationTypeRegistry =
            OperationIdRegistry.ToDictionary(x => x.Value, x => x.Key);

        public static AggregateOperation CreateOperation(this Guid guid, int entityTypeId, long entityId)
        {
            Type operationType;
            if (OperationIdRegistry.TryGetValue(guid, out operationType))
            {
                var entityType = EntityType.Instance.Parse(entityTypeId);
                return (AggregateOperation)Activator.CreateInstance(operationType,
                                                                    EntityTypeMap<CustomerIntelligenceContext>.AsEntityType(entityType),
                                                                    entityId);
            }

            throw new ArgumentException(string.Format("Unknown operation id {0}", guid), "guid");
        }

        public static Guid GetIdentity(this AggregateOperation operation)
        {
            Guid guid;
            if (OperationTypeRegistry.TryGetValue(operation.GetType(), out guid))
            {
                return guid;
            }

            throw new ArgumentException(string.Format("Unknown operation type {0}", operation.GetType().Name), "operationType");
        }
    }
}
