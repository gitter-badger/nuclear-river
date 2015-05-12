using System;
using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
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

        public IQueryable<Account> Accounts
        {
            get { return _context.GetTable<Account>(); }
        }

        public IQueryable<Category> Categories
        {
            get { return _context.GetTable<Category>(); }
        }

        public IQueryable<CategoryGroup> CategoryGroups
        {
            get { return _context.GetTable<CategoryGroup>(); }
        }

        public IQueryable<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits
        {
            get { return _context.GetTable<BranchOfficeOrganizationUnit>(); }
        }

        public IQueryable<CategoryFirmAddress> CategoryFirmAddresses
        {
            get { return _context.GetTable<CategoryFirmAddress>(); }
        }

        public IQueryable<CategoryOrganizationUnit> CategoryOrganizationUnits
        {
            get { return _context.GetTable<CategoryOrganizationUnit>(); }
        }

        public IQueryable<Client> Clients
        {
            get { return _context.GetTable<Client>(); }
        }

        public IQueryable<Contact> Contacts
        {
            get { return _context.GetTable<Contact>(); }
        }

        public IQueryable<Firm> Firms
        {
            get { return _context.GetTable<Firm>(); }
        }

        public IQueryable<FirmAddress> FirmAddresses
        {
            get { return _context.GetTable<FirmAddress>(); }
        }

        public IQueryable<FirmContact> FirmContacts
        {
            get { return _context.GetTable<FirmContact>(); }
        }

        public IQueryable<LegalPerson> LegalPersons
        {
            get { return _context.GetTable<LegalPerson>(); }
        }

        public IQueryable<Order> Orders
        {
            get { return _context.GetTable<Order>(); }
        }

        public IQueryable<Project> Projects
        {
            get { return _context.GetTable<Project>(); }
        }

        public IQueryable<Territory> Territories
        {
            get { return _context.GetTable<Territory>(); }
        }
    }
}