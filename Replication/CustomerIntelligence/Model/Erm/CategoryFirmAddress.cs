using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class CategoryFirmAddress : IIdentifiableObject, IErmObject
    {
        public CategoryFirmAddress()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public long CategoryId { get; set; }

        public long FirmAddressId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public override bool Equals(object obj)
        {
            return obj is CategoryFirmAddress && IdentifiableObjectEqualityComparer<CategoryFirmAddress>.Default.Equals(this, (CategoryFirmAddress)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<CategoryFirmAddress>.Default.GetHashCode(this);
        }
    }
}