using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class Client : IIdentifiable, ICustomerIntelligenceObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long CategoryGroupId { get; set; }

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