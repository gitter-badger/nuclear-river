using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class Contact : IErmObject
    {
        public Contact()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public int Role { get; set; }

        public bool IsFired { get; set; }

        public string MainPhoneNumber { get; set; }

        public string AdditionalPhoneNumber { get; set; }

        public string MobilePhoneNumber { get; set; }

        public string HomePhoneNumber { get; set; }

        public string Website { get; set; }

        public long ClientId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

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