using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class CategoryOrganizationUnit : IIdentifiableObject, IFactObject
    {
        public long Id { get; set; }

        public long CategoryId { get; set; }

        public long CategoryGroupId { get; set; }

        public long OrganizationUnitId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is CategoryOrganizationUnit && IdentifiableObjectEqualityComparer<CategoryOrganizationUnit>.Default.Equals(this, (CategoryOrganizationUnit)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<CategoryOrganizationUnit>.Default.GetHashCode(this);
        }
    }
}