﻿using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Statistics
{
    public sealed class FirmCategoryStatistics : ICustomerIntelligenceAggregatePart, IAggregateValueObject
    {
        public long ProjectId { get; set; }

        public long FirmId { get; set; }

        public long CategoryId { get; set; }

        public int? Hits { get; set; }

        public int? Shows { get; set; }

        public float? AdvertisersShare { get; set; }

        public int? FirmCount { get; set; }
    }
}