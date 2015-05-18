using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class LegalPerson : IErmObject
    {
        public LegalPerson()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public long? ClientId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public override bool Equals(object obj)
        {
            return obj is LegalPerson && IdentifiableObjectEqualityComparer<LegalPerson>.Default.Equals(this, (LegalPerson)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<LegalPerson>.Default.GetHashCode(this);
        }
    }
}