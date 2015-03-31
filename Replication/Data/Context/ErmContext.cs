using System;
using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.Data.Context
{
    public sealed class ErmContext : IErmContext
    {
        private readonly IDataContext _context;

        public ErmContext(IDataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
        }

        public IQueryable<Erm.Account> Accounts
        {
            get
            {
                return from account in _context.GetTable<Erm.Account>()
                       where account.IsActive && !account.IsDeleted
                       select account;
            }
        }

        public IQueryable<Erm.CategoryFirmAddress> CategoryFirmAddresses
        {
            get
            {
                return from categoryFirmAddress in _context.GetTable<Erm.CategoryFirmAddress>()
                       where categoryFirmAddress.IsActive && !categoryFirmAddress.IsDeleted
                       select categoryFirmAddress;
            }
        }

        public IQueryable<Erm.CategoryOrganizationUnit> CategoryOrganizationUnits
        {
            get
            {
                return from categoryOrganizationUnit in _context.GetTable<Erm.CategoryOrganizationUnit>()
                       where categoryOrganizationUnit.IsActive && !categoryOrganizationUnit.IsDeleted
                       select categoryOrganizationUnit;
            }
        }

        public IQueryable<Erm.Firm> Firms
        {
            get
            {
                return from firm in _context.GetTable<Erm.Firm>()
                       where firm.IsActive && !firm.IsDeleted && !firm.ClosedForAscertainment
                       select firm;
            }
        }

        public IQueryable<Erm.FirmAddress> FirmAddresses
        {
            get
            {
                return from firmAddress in _context.GetTable<Erm.FirmAddress>()
                       where firmAddress.IsActive && !firmAddress.IsDeleted && !firmAddress.ClosedForAscertainment
                       select firmAddress;
            }
        }

        public IQueryable<Erm.FirmContact> FirmContacts
        {
            get
            {
                return from firmContact in _context.GetTable<Erm.FirmContact>()
                       select firmContact;
            }
        }

        public IQueryable<Erm.LegalPerson> LegalPersons
        {
            get
            {
                return from legalPerson in _context.GetTable<Erm.LegalPerson>()
                       where legalPerson.IsActive && !legalPerson.IsDeleted
                       select legalPerson;
            }
        }

        public IQueryable<Erm.Client> Clients
        {
            get
            {
                return from client in _context.GetTable<Erm.Client>()
                       where client.IsActive && !client.IsDeleted
                       select client;
            }
        }

        public IQueryable<Erm.Contact> Contacts
        {
            get
            {
                return from contact in _context.GetTable<Erm.Contact>()
                       where contact.IsActive && !contact.IsDeleted
                       select contact;
            }
        }

        public IQueryable<Erm.Order> Orders
        {
            get
            {
                return from order in _context.GetTable<Erm.Order>()
                       where order.IsActive && !order.IsDeleted
                       select order;
            }
        }
    }
}