using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.DTO
{
    public sealed class FirmStatisticsDto : IStatisticsDto
    {
        public long ProjectId { get; set; }

        public IReadOnlyCollection<FirmDto> Firms { get; set; }

        public class FirmDto
        {
            public long FirmId { get; set; }

            public IReadOnlyCollection<CategoryDto> Categories { get; set; }

            public class CategoryDto
            {
                public long CategoryId { get; set; }
                public long Hits { get; set; }
                public long Shows { get; set; }
            }
        }
    }
}