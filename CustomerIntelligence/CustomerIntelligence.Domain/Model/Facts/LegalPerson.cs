using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class LegalPerson : IFactObject
    {
        public long Id { get; set; }

        public long ClientId { get; set; }

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