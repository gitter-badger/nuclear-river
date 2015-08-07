using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class Project : IErmFactObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long OrganizationUnitId { get; set; }

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