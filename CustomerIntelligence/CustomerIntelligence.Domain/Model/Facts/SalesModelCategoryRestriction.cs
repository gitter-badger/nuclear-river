using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class SalesModelCategoryRestriction : IFactObject
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public long ProjectId { get; set; }
        public int SalesModel { get; set; }
    }
}