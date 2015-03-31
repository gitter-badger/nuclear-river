using System;
using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;

namespace NuClear.AdvancedSearch.Replication.Data.Context
{
    public sealed class FactsContext : IFactsContext
    {
        private readonly IDataContext _context;

        public FactsContext(IDataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
        }

        public IQueryable<Fact.Account> Accounts
        {
            get
            {
                return _context.GetTable<Fact.Account>();
            }
        }

        public IQueryable<Fact.CategoryFirmAddress> CategoryFirmAddresses
        {
            get
            {
                return _context.GetTable<Fact.CategoryFirmAddress>();
            }
        }

        public IQueryable<Fact.CategoryOrganizationUnit> CategoryOrganizationUnits
        {
            get
            {
                return _context.GetTable<Fact.CategoryOrganizationUnit>();
            }
        }

        public IQueryable<Fact.Client> Clients
        {
            get
            {
                return _context.GetTable<Fact.Client>();
            }
        }

        public IQueryable<Fact.Contact> Contacts
        {
            get
            {
                return _context.GetTable<Fact.Contact>();
            }
        }

        public IQueryable<Fact.Firm> Firms
        {
            get
            {
                return _context.GetTable<Fact.Firm>();
            }
        }

        public IQueryable<Fact.FirmAddress> FirmAddresses
        {
            get
            {
                return _context.GetTable<Fact.FirmAddress>();
            }
        }

        public IQueryable<Fact.FirmContact> FirmContacts
        {
            get
            {
                return _context.GetTable<Fact.FirmContact>();
            }
        }

        public IQueryable<Fact.LegalPerson> LegalPersons
        {
            get
            {
                return _context.GetTable<Fact.LegalPerson>();
            }
        }

        public IQueryable<Fact.Order> Orders
        {
            get
            {
                return _context.GetTable<Fact.Order>();
            }
        }
    }
}