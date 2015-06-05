using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class Category : IErmFactObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public long? ParentId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Category && IdentifiableObjectEqualityComparer<Category>.Default.Equals(this, (Category)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<Category>.Default.GetHashCode(this);
        }
    }
}