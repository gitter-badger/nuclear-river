using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class FirmAccount : IIdentifiable
    {
        public long Id { get; set; }

        public long AccountId { get; set; }

        public long FirmId { get; set; }

        public decimal Balance { get; set; }

        private bool Equals(FirmAccount other)
        {
            return Id == other.Id && AccountId == other.AccountId && FirmId == other.FirmId && Balance == other.Balance;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is FirmAccount && Equals((FirmAccount)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ AccountId.GetHashCode();
                hashCode = (hashCode * 397) ^ FirmId.GetHashCode();
                hashCode = (hashCode * 397) ^ Balance.GetHashCode();
                return hashCode;
            }
        }
    }
}