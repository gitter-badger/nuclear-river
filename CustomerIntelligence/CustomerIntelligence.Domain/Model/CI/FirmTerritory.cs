namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class FirmTerritory : ICustomerIntelligenceObject
    {
        public long FirmId { get; set; }

        public long TerritoryId { get; set; }

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

            return obj is FirmTerritory && Equals((FirmTerritory)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FirmId.GetHashCode() * 397) ^ TerritoryId.GetHashCode();
            }
        }

        private bool Equals(FirmTerritory other)
        {
            return FirmId == other.FirmId && TerritoryId == other.TerritoryId;
        }
    }
}