using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class BranchOfficeOrganizationUnit : IErmFactObject
    {
        public long Id { get; set; }

        public long OrganizationUnitId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is BranchOfficeOrganizationUnit && IdentifiableObjectEqualityComparer<BranchOfficeOrganizationUnit>.Default.Equals(this, (BranchOfficeOrganizationUnit)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<BranchOfficeOrganizationUnit>.Default.GetHashCode(this);
        }
    }
}