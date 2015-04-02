using System;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class Firm : IIdentifiableObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? LastDisqualifiedOn { get; set; }

        public DateTimeOffset? LastDistributedOn { get; set; }

        public bool HasPhone { get; set; }

        public bool HasWebsite { get; set; }

        public int AddressCount { get; set; }

        public long CategoryGroupId { get; set; }

        public long? ClientId { get; set; }

        public long OrganizationUnitId { get; set; }

        public long TerritoryId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Firm && IdentifiableObjectEqualityComparer<Firm>.Default.Equals(this, (Firm)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<Firm>.Default.GetHashCode(this);
        }
    }
}