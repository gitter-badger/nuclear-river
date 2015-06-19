using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class FirmCategoryStatistics : ICustomerIntelligenceObject
    {
        public long OrganizationUnitId { get; set; }

        public long CategoryId { get; set; }

        public float AdvertisersShare { get; set; }

        public long FirmCount { get; set; }

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

            return obj is FirmCategoryStatistics && Equals((FirmCategoryStatistics)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (OrganizationUnitId.GetHashCode() * 397) ^ CategoryId.GetHashCode();
            }
        }

        private bool Equals(FirmCategoryStatistics other)
        {
            return OrganizationUnitId == other.OrganizationUnitId && CategoryId == other.CategoryId;
        }
    }
}