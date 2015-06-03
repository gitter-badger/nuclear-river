using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Bit;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public class BitContext : IBitContext
    {
        private readonly IDataContext _context;

        public BitContext(IDataContext context)
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