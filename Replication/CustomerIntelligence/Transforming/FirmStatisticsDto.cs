using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public class FirmStatisticsDto
    {
        public long ProjectId { get; set; }

        public IEnumerable<FirmDto> Firms { get; set; }

        public class FirmDto
        {
            public long FirmId { get; set; }
            public long CategoryId { get; set; }
            public long Hits { get; set; }
            public long Shows { get; set; }
        }
    }
}