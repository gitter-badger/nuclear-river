namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class BranchOfficeOrganizationUnit : IErmFactObject
    {
        public long Id { get; set; }

        public long OrganizationUnitId { get; set; }
    }
}