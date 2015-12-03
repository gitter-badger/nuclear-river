using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class CategoryGroup : IFactObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public float Rate { get; set; }
    }
}