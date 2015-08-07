using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class FirmCategory : IBitFactObject
    {
        public long ProjectId { get; set; }
        public long FirmId { get; set; }
        public long CategoryId { get; set; }
    }
}