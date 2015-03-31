using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class Account : IIdentifiable
    {
        public Account()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public decimal Balance { get; set; }

        public long LegalPersonId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}