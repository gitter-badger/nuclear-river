using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class Project : IIdentifiableObject, ICustomerIntelligenceObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Project && IdentifiableObjectEqualityComparer<Project>.Default.Equals(this, (Project)obj);
        }

        public override int GetHashCode()
        {
            return IdentifiableObjectEqualityComparer<Project>.Default.GetHashCode(this);
        }
    }
}