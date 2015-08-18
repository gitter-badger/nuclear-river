using System.Linq;

using LinqToDB;

using FirmCategoryStatistics = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.FirmCategoryStatistics;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
    using Facts = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

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
                var firmCounts = _bitContext.GetTable<Facts.FirmCategory>()
                                            .GroupBy(x => new { x.ProjectId, x.CategoryId })
                                            .Select(x => new { x.Key.ProjectId, x.Key.CategoryId, Count = x.Count() });

                return from firm in _bitContext.GetTable<Facts.FirmCategory>()
                       from firmStatistics in _bitContext.GetTable<Facts.FirmCategoryStatistics>().Where(x => x.FirmId == firm.FirmId && x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId).DefaultIfEmpty()
                       from categoryStatistics in _bitContext.GetTable<Facts.ProjectCategoryStatistics>().Where(x => x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId).DefaultIfEmpty()
                       from firmCount in firmCounts.Where(x => x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId).DefaultIfEmpty()
                       select new CI.FirmCategoryStatistics
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