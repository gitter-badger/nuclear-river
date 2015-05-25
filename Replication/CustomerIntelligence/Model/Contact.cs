using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class Contact : IIdentifiable, ICustomerIntelligenceObject
    {
        public long Id { get; set; }

        public int Role { get; set; }

        public bool IsFired { get; set; }

        public long ClientId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Contact && IdentifiableObjectEqualityComparer<Contact>.Default.Equals(this, (Contact)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<Contact>.Default.GetHashCode(this);
        }
    }
}