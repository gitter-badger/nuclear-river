using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public sealed class FirmCategory : IFactStatisticsObject
    {
        public long ProjectId { get; set; }
        public long FirmId { get; set; }
        public long CategoryId { get; set; }
    }
}