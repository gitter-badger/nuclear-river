using System.Collections.Generic;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
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
    }
}