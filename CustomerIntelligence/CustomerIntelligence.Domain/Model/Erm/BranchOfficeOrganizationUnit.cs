using NuClear.Replication.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Erm
{
    public sealed class BranchOfficeOrganizationUnit : IErmObject
    {
        public BranchOfficeOrganizationUnit()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public long OrganizationUnitId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

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