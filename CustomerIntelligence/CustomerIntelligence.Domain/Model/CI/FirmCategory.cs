using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class FirmCategory : ICustomerIntelligenceObject
    {
        public long FirmId { get; set; }

        public long CategoryId { get; set; }
    }
}