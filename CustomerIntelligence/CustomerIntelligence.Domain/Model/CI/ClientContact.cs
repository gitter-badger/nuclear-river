namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class ClientContact : ICustomerIntelligenceObject
    {
        public long ClientId { get; set; }

        public long ContactId { get; set; }

        public int Role { get; set; }

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
            return obj is ClientContact && Equals((ClientContact)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ClientId.GetHashCode();
                hashCode = (hashCode * 397) ^ ContactId.GetHashCode();
                hashCode = (hashCode * 397) ^ Role;
                return hashCode;
            }
        }

        private bool Equals(ClientContact other)
        {
            return ClientId == other.ClientId && ContactId == other.ContactId && Role == other.Role;
        }
    }
}