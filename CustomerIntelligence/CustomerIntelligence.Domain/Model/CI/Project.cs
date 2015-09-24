using NuClear.Replication.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class Project : IIdentifiable, ICustomerIntelligenceObject
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