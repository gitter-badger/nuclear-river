using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class FirmAddress : IIdentifiableObject, IFactObject
    {
        public long Id { get; set; }

        public long FirmId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is FirmAddress && IdentifiableObjectEqualityComparer<FirmAddress>.Default.Equals(this, (FirmAddress)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<FirmAddress>.Default.GetHashCode(this);
        }
    }
}