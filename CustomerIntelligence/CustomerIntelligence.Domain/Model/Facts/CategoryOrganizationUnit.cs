using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class CategoryOrganizationUnit : IFactObject
    {
        public long Id { get; set; }

        public long CategoryId { get; set; }

        public long CategoryGroupId { get; set; }

        public long OrganizationUnitId { get; set; }
    }
}