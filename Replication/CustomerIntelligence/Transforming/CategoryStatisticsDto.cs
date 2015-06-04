using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public class CategoryStatisticsDto
    {
        public long ProjectId { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; }

        public class CategoryDto
        {
            public long CategoryId { get; set; }

            public int AdvertisersCount { get; set; }
        }
    }
}