using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class Account : IIdentifiable
    {
        public long Id { get; set; }

        public decimal Balance { get; set; }

        public long LegalPersonId { get; set; }
    }
}