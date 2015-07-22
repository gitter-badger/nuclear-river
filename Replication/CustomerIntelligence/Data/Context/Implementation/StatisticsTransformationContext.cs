using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

using FirmCategoryStatistics = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.FirmCategoryStatistics;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public sealed class StatisticsTransformationContext : IStatisticsContext
    {
        private readonly IDataContext _bitContext;

        public StatisticsTransformationContext(IDataContext bitContext)
        {
            _bitContext = bitContext;
        }

        public IQueryable<FirmCategoryStatistics> FirmCategoryStatistics
        {
            get
            {
                var firmCounts = _bitContext.GetTable<FirmCategory>().GroupBy(x => new { x.ProjectId, x.CategoryId }).Select(x => new { x.Key.ProjectId, x.Key.CategoryId, Count = x.Count() });

                return from firm in _bitContext.GetTable<FirmCategory>()
                       from firmStatistics in _bitContext.GetTable<Model.Facts.FirmCategoryStatistics>().Where(x => x.FirmId == firm.FirmId && x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId).DefaultIfEmpty()
                       from categoryStatistics in _bitContext.GetTable<ProjectCategoryStatistics>().Where(x => x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId).DefaultIfEmpty()
                       from firmCount in firmCounts.Where(x => x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId).DefaultIfEmpty()
                       select new Model.FirmCategoryStatistics
                              {
                                  ProjectId = firm.ProjectId,
                                  FirmId = firm.FirmId,
                                  CategoryId = firm.CategoryId,
                                  Hits = firmStatistics.Hits,
                                  Shows = firmStatistics.Shows,
                                  FirmCount = firmCount.Count,
                                  AdvertisersShare = categoryStatistics.AdvertisersCount / firmCount.Count
                              };
            }
        }
    }
}