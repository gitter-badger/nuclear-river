using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Statistics
{
    public sealed class FirmCategoryStatistics : ICustomerIntelligenceAggregatePart, IAggregateValueObject
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