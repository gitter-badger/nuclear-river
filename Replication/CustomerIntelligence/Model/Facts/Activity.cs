using System;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class Activity : IErmFactObject
    {
        public long Id { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }

        public long? FirmId { get; set; }

        public long? ClientId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Activity && IdentifiableObjectEqualityComparer<Activity>.Default.Equals(this, (Activity)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<Activity>.Default.GetHashCode(this);
        }
    }
}