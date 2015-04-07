using System;

using NuClear.AdvancedSearch.Replication.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations
{
    public abstract class AggregateOperation : IOperation
    {
        protected AggregateOperation(Type aggregateType, long aggregateId)
        {
            if (aggregateType == null)
            {
                throw new ArgumentNullException("aggregateType");
            }

            AggregateType = aggregateType;
            AggregateId = aggregateId;
        }

        public Type AggregateType { get; private set; }

        public long AggregateId { get; private set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is FactOperation && Equals((FactOperation)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (AggregateType != null ? AggregateType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AggregateId.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(FactOperation other)
        {
            return AggregateType == other.FactType && AggregateId == other.FactId;
        }
    }
}