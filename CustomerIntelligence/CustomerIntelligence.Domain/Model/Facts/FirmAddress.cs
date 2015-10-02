using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class FirmAddress : IFactObject
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