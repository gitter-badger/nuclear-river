using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class FirmAccount : IIdentifiable
    {
        public long Id { get; set; }

        public long AccountId { get; set; }

        public long FirmId { get; set; }

        public decimal Balance { get; set; }
    }
}