using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class FirmAddress : IIdentifiableObject
    {
        public FirmAddress()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public long FirmId { get; set; }

        public bool ClosedForAscertainment { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

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