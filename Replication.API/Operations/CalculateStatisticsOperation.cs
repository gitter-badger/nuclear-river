namespace NuClear.AdvancedSearch.Replication.API.Operations
{
    public class CalculateStatisticsOperation : IOperation
    {
        public long ProjectId { get; set; }
        public long? CategoryId { get; set; }

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

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((CalculateStatisticsOperation)obj);
        }

        public override int GetHashCode()
        {
            return (CategoryId.GetHashCode() * 397) ^ ProjectId.GetHashCode();
        }

        private bool Equals(CalculateStatisticsOperation other)
        {
            return CategoryId == other.CategoryId && ProjectId == other.ProjectId;
        }

        public override string ToString()
        {
            return string.Format("{0}(Project:{1}, Category:{2})", GetType().Name, ProjectId, CategoryId);
        }
    }
}
