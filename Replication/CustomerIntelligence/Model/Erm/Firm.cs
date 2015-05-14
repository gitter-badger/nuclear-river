using System;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class Firm : IIdentifiableObject, IErmObject
    {
        public Firm()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? LastDisqualifyTime { get; set; }

        public long? ClientId { get; set; }

        public long OrganizationUnitId { get; set; }

        public long OwnerId { get; set; }

        public long TerritoryId { get; set; }

        public bool ClosedForAscertainment { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

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