using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class Category : IIdentifiableObject
    {
        public Category()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public int Level { get; set; }

        public long? ParentId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

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