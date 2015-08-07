using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public class ProjectCategoryStatistics : IBitFactObject
    {
        public long ProjectId { get; set; }

        public long CategoryId { get; set; }

        public long AdvertisersCount { get; set; }
    }
}