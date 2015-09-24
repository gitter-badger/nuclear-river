using NuClear.Replication.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public class ProjectCategoryStatistics : IFactStatisticsObject
    {
        public long ProjectId { get; set; }

        public long CategoryId { get; set; }

        public long AdvertisersCount { get; set; }
    }
}