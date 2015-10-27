using System;
using System.Linq;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
    using Facts = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

    public sealed class StatisticsTransformationContext : IStatisticsContext
    {
        private readonly IBitFactsContext _bitContext;

        public StatisticsTransformationContext(IBitFactsContext bitContext)
        {
            _bitContext = bitContext;
        }

        public IQueryable<CI::FirmCategoryStatistics> FirmCategoryStatistics
        {
            get
            {
                var firmCounts = _bitContext.FirmCategory.GroupBy(x => new { x.ProjectId, x.CategoryId }).Select(x => new { x.Key.ProjectId, x.Key.CategoryId, Count = x.Count() });

                return from firm in _bitContext.FirmCategory
                       from firmStatistics in _bitContext.FirmStatistics.Where(x => x.FirmId == firm.FirmId && x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId).DefaultIfEmpty(new Facts::FirmCategoryStatistics())
                       from categoryStatistics in _bitContext.CategoryStatistics.Where(x => x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId).DefaultIfEmpty(new Facts::ProjectCategoryStatistics())
                       from firmCount in firmCounts.Where(x => x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId).DefaultIfEmpty()
                       select new CI::FirmCategoryStatistics
                           {
                               ProjectId = firm.ProjectId,
                               FirmId = firm.FirmId,
                               CategoryId = firm.CategoryId,
                               Hits = firmStatistics != null ? firmStatistics.Hits : 0,
                               Shows = firmStatistics != null ? firmStatistics.Shows : 0,
                               FirmCount = firmCount.Count,
                               AdvertisersShare = Math.Min(1, (float)categoryStatistics.AdvertisersCount / firmCount.Count)
                           };
            }
        }
    }
}