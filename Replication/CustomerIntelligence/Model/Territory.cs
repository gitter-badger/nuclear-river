using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class Territory : IIdentifiable, ICustomerIntelligenceObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long ProjectId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Territory && IdentifiableObjectEqualityComparer<Territory>.Default.Equals(this, (Territory)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<Territory>.Default.GetHashCode(this);
        }
    }
}