using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Model;

namespace NuClear.CustomerIntelligence.Domain.DTO
{
    public sealed class CategoryStatisticsDto : IBitDto
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