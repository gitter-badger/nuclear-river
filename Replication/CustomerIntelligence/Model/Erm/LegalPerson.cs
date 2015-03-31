using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class LegalPerson : IIdentifiable
    {
        public LegalPerson()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public long? ClientId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}