using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class LegalPerson : IIdentifiable
    {
        public long Id { get; set; }

        public long? ClientId { get; set; }
    }
}