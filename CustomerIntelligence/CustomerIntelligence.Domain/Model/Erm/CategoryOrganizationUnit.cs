using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Erm
{
    public sealed class CategoryOrganizationUnit : IErmObject
    {
        public CategoryOrganizationUnit()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public long CategoryId { get; set; }

        public long CategoryGroupId { get; set; }

        public long OrganizationUnitId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}