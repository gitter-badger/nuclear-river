using System.Linq;

using LinqToDB;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

    public sealed class StatisticsContext : IStatisticsContext
    {
        private readonly IDataContext _context;

        public StatisticsContext(IDataContext context)
        {
            _context = context;
        }

        public IQueryable<CI.FirmCategoryStatistics> FirmCategoryStatistics
        {
            get
            {
                return from firm in _context.GetTable<CI.Firm>()
                       join statistics in _context.GetTable<CI.FirmCategoryStatistics>() on firm.Id equals statistics.FirmId
                       select new CI.FirmCategoryStatistics
                              {
                                  ProjectId = firm.ProjectId,

                                  FirmId = statistics.FirmId,
                                  CategoryId = statistics.CategoryId,
                                  AdvertisersShare = statistics.AdvertisersShare,
                                  FirmCount = statistics.FirmCount,
                                  Hits = statistics.Hits,
                                  Shows = statistics.Shows,
                              };
            }
        }
    }
}