using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public class BitFactsContext : IBitFactsContext
    {
        private readonly IDataContext _context;

        public BitFactsContext(IDataContext context)
        {
            _context = context;
        }

        public IQueryable<FirmStatistics> FirmStatistics
        {
            get { return _context.GetTable<FirmStatistics>(); }
        }

        public IQueryable<CategoryStatististics> CategoryStatististics
        {
            get { return _context.GetTable<CategoryStatististics>(); }
        }
    }
}