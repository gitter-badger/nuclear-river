using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class CategoryStatisticsDto : ICorporateBusDto
    {
        public long ProjectId { get; set; }

        public IEnumerable<CategoryDto> Categories { get; set; }

        public class CategoryDto
        {
            public long CategoryId { get; set; }

            public long AdvertisersCount { get; set; }
        }
    }
}