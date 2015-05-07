using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
{
    public sealed class ProjectCategory : IObject
    {
        public long ProjectId { get; set; }
        
        public long CategoryId { get; set; }

        public float AdvertisersShare { get; set; }

        public long FirmCount { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is ProjectCategory && Equals((ProjectCategory)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ProjectId.GetHashCode() * 397) ^ CategoryId.GetHashCode();
            }
        }

        private bool Equals(ProjectCategory other)
        {
            return ProjectId == other.ProjectId && CategoryId == other.CategoryId;
        }
    }
}