namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class FirmCategory
    {
        public long FirmId { get; set; }

        public long CategoryId { get; set; }

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
            return obj is FirmCategory && Equals((FirmCategory)obj);
        }

        private bool Equals(FirmCategory other)
        {
            return FirmId == other.FirmId && CategoryId == other.CategoryId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FirmId.GetHashCode() * 397) ^ CategoryId.GetHashCode();
            }
        }
    }
}