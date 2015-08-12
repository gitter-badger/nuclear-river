using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class FirmStatisticsDto : ICorporateBusDto
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