using System;
using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;

namespace NuClear.AdvancedSearch.Replication.Data.Context
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

        public IQueryable<CustomerIntelligence.FirmAccount> Accounts
        {
            get
            {
                return _context.GetTable<CustomerIntelligence.FirmAccount>();
            }
        }
    }
}