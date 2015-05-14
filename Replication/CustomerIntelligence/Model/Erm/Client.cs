using System;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class Client : IIdentifiableObject, IErmObject
    {
        public Client()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset? LastDisqualifyTime { get; set; }

        public string MainPhoneNumber { get; set; }

        public string AdditionalPhoneNumber1 { get; set; }

        public string AdditionalPhoneNumber2 { get; set; }

        public string Website { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Client && IdentifiableObjectEqualityComparer<Client>.Default.Equals(this, (Client)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<Client>.Default.GetHashCode(this);
        }
    }
}