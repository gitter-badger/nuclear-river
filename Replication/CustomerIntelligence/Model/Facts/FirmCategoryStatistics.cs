using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public class FirmCategoryStatistics : IBitFactObject
    {
        public long FirmId { get; set; }

        public long ProjectId { get; set; }

        public long CategoryId { get; set; }

        public long Hits { get; set; }

        public long Shows { get; set; }
    }
}