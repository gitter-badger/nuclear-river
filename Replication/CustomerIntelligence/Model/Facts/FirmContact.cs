using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class FirmContact : IErmFactObject
    {
        public long Id { get; set; }

        public bool HasPhone { get; set; }
        
        public bool HasWebsite { get; set; }

        public long FirmAddressId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is FirmContact && IdentifiableObjectEqualityComparer<FirmContact>.Default.Equals(this, (FirmContact)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<FirmContact>.Default.GetHashCode(this);
        }
    }
}