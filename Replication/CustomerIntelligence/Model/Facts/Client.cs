using System;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class Client : IIdentifiableObject, IFactObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset? LastDisqualifiedOn { get; set; }

        public bool HasPhone { get; set; }

        public bool HasWebsite { get; set; }

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