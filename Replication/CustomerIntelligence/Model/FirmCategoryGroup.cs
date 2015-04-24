using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class FirmCategoryGroup : IObject
    {
        public long FirmId { get; set; }

        public long CategoryGroupId { get; set; }

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
            return obj is FirmCategory && Equals((FirmCategory)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FirmId.GetHashCode() * 397) ^ CategoryGroupId.GetHashCode();
            }
        }

        private bool Equals(FirmCategory other)
        {
            return FirmId == other.FirmId && CategoryGroupId == other.CategoryId;
        }
    }
}