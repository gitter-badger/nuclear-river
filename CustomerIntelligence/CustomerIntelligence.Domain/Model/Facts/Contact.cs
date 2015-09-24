using NuClear.Replication.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Contact : IFactObject
    {
        public long Id { get; set; }

        public int Role { get; set; }

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