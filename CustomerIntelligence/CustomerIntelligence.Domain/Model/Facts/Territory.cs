using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Territory : IFactObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long OrganizationUnitId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Territory && IdentifiableObjectEqualityComparer<Territory>.Default.Equals(this, (Territory)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<Territory>.Default.GetHashCode(this);
        }
    }
}