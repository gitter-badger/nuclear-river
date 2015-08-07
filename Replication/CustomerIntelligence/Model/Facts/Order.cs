using System;

using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class Order : IErmFactObject
    {
        public long Id { get; set; }

        public DateTimeOffset EndDistributionDateFact { get; set; }

        public long FirmId { get; set; }

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