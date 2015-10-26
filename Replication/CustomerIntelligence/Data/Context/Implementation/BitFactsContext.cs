using System.Linq;

using LinqToDB;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    using Facts = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

    public class BitFactsContext : IBitFactsContext
    {
        private readonly IDataContext _context;

        public BitFactsContext(IDataContext context)
        {
            _context = context;
        }

        public IQueryable<Facts::FirmCategory> FirmCategory 
            => _context.GetTable<Facts.FirmCategory>();

        public IQueryable<Facts::FirmCategoryStatistics> FirmStatistics
            => _context.GetTable<Facts.FirmCategoryStatistics>();

        public IQueryable<Facts::ProjectCategoryStatistics> CategoryStatistics 
            => _context.GetTable<Facts.ProjectCategoryStatistics>();
    }
}