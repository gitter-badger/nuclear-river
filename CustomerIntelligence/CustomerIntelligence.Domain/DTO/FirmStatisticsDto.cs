using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Model;

namespace NuClear.CustomerIntelligence.Domain.DTO
{
    public sealed class FirmStatisticsDto : IBitDto
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
                public int Hits { get; set; }
                public int Shows { get; set; }
            }
        }
    }
}