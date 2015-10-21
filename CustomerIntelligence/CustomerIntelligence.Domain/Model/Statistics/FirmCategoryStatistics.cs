using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Statistics
{
    public sealed class FirmCategoryStatistics : ICustomerIntelligenceObject
    {
        public long ProjectId { get; set; }

        public long FirmId { get; set; }

        public long CategoryId { get; set; }

        public long? Hits { get; set; }

        public long? Shows { get; set; }

        public float? AdvertisersShare { get; set; }

        public long? FirmCount { get; set; }

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
                return (FirmId.GetHashCode() * 397) ^ CategoryId.GetHashCode();
            }
        }

        private bool Equals(FirmCategoryStatistics other)
        {
            return FirmId == other.FirmId && CategoryId == other.CategoryId;
        }

        public class FullEqualityComparer : IEqualityComparer<FirmCategoryStatistics>
        {
            public bool Equals(FirmCategoryStatistics x, FirmCategoryStatistics y)
            {
                return x.ProjectId == y.ProjectId
                       && x.FirmId == y.FirmId
                       && x.CategoryId == y.CategoryId
                       && x.Hits == y.Hits
                       && x.Shows == y.Shows
                       && x.AdvertisersShare == y.AdvertisersShare
                       && x.FirmCount == y.FirmCount;
            }

            public int GetHashCode(FirmCategoryStatistics obj)
            {
                unchecked
                {
                    var code = obj.ProjectId.GetHashCode();
                    code = (code * 397) ^ obj.FirmId.GetHashCode();
                    code = (code * 397) ^ obj.CategoryId.GetHashCode();
                    code = (code * 397) ^ obj.Hits.GetHashCode();
                    code = (code * 397) ^ obj.Shows.GetHashCode();
                    code = (code * 397) ^ obj.AdvertisersShare.GetHashCode();
                    code = (code * 397) ^ obj.FirmCount.GetHashCode();
                    return code;
                }
            }
        }
    }
}