using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class Project : IErmObject
    {
        public Project()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public long OrganizationUnitId { get; set; }

        public bool IsActive { get; set; }

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