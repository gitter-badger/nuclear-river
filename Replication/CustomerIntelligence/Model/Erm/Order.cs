using System;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class Order : IIdentifiableObject
    {
        public Order()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public DateTimeOffset EndDistributionDateFact { get; set; }

        public int WorkflowStepId { get; set; }

        public long FirmId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Order && IdentifiableObjectEqualityComparer<Order>.Default.Equals(this, (Order)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<Order>.Default.GetHashCode(this);
        }
    }
}