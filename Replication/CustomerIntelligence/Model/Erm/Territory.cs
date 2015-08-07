using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class Territory : IErmObject
    {
        public Territory()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public long OrganizationUnitId { get; set; }

        public bool IsActive { get; set; }

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