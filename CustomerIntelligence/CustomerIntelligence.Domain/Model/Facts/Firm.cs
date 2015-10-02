using System;

using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Firm : IFactObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? LastDisqualifiedOn { get; set; }

        public long? ClientId { get; set; }

        public long OrganizationUnitId { get; set; }

        public long OwnerId { get; set; }

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