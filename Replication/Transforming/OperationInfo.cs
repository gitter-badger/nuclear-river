using System;

namespace NuClear.AdvancedSearch.Replication.Transforming
{
    public sealed class OperationInfo
    {
        public OperationInfo(Operation operation, Type entityType, long entityId)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            EntityType = entityType;
            EntityId = entityId;
            Operation = operation;
        }

        public Type EntityType { get; private set; }

        public long EntityId { get; private set; }

        public Operation Operation { get; private set; }
    }
}