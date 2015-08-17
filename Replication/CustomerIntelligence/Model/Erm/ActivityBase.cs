using System;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class ActivityBase<T> : IErmObject
    {
        public long Id { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ActivityBase<T> && IdentifiableObjectEqualityComparer<ActivityBase<T>>.Default.Equals(this, (ActivityBase<T>)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<ActivityBase<T>>.Default.GetHashCode(this);
        }
    }
}
