using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public class FirmStatistics : IObject
    {
        public long FirmId { get; set; }

        public long ProjectId { get; set; }

        public long CategoryId { get; set; }

        public int Hits { get; set; }

        public int Shows { get; set; }
    }
}