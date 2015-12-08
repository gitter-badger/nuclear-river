using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class BranchOfficeOrganizationUnit : IFactObject
    {
        public long Id { get; set; }

        public long OrganizationUnitId { get; set; }
    }
}