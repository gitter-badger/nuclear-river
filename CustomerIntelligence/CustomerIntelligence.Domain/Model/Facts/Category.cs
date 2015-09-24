using NuClear.Replication.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Category : IFactObject
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