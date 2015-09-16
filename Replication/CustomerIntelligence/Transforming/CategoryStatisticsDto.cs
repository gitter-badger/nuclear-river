using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class CategoryStatisticsDto : IStatisticsDto
    {
        public long ProjectId { get; set; }

        public IReadOnlyCollection<CategoryDto> Categories { get; set; }

        public class CategoryDto
        {
            public long CategoryId { get; set; }

            public long AdvertisersCount { get; set; }
        }
    }
}