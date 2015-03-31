using System;
using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public class CustomerIntelligenceContext
    {
        private readonly IDataContext _context;

        public CustomerIntelligenceContext(IDataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
        }

        public IQueryable<FirmAccount> Accounts
        {
            get
            {
                return _context.GetTable<FirmAccount>();
            }
        }
    }
}