using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class FirmBalance : ICustomerIntelligenceObject
    {
        public long ProjectId { get; set; }

        public long FirmId { get; set; }

        public long AccountId { get; set; }

        public decimal Balance { get; set; }
    }
}