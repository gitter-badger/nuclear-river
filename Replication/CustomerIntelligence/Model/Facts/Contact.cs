using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class Contact : IErmFactObject
    {
        public long Id { get; set; }

        public int Role { get; set; }

        public bool IsFired { get; set; }

        public bool HasPhone { get; set; }

        public bool HasWebsite { get; set; }

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