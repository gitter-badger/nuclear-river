﻿namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public class ProjectCategoryStatistics : IBitFactObject
    {
        public long ProjectId { get; set; }

        public long CategoryId { get; set; }

        public long AdvertisersCount { get; set; }
    }
}