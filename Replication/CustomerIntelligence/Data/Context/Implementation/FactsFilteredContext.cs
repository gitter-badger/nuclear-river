using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public sealed class FactsFilteredContext : IFactsContext
    {
        private readonly IFactsContext _context;
        private readonly Type _elementType;
        private readonly IEnumerable<long> _ids;

        public FactsFilteredContext(IFactsContext context, Type elementType, IEnumerable<long> ids)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _context = context;
            _elementType = elementType;
            _ids = ids;
        }

        public IQueryable<Account> Accounts
        {
            get
            {
                return Filter(_context.Accounts);
            }
        }

        public IQueryable<CategoryFirmAddress> CategoryFirmAddresses
        {
            get
            {
                return Filter(_context.CategoryFirmAddresses);
            }
        }

        public IQueryable<CategoryOrganizationUnit> CategoryOrganizationUnits
        {
            get
            {
                return Filter(_context.CategoryOrganizationUnits);
            }
        }

        public IQueryable<Client> Clients
        {
            get
            {
                return Filter(_context.Clients);
            }
        }

        public IQueryable<Contact> Contacts
        {
            get
            {
                return Filter(_context.Contacts);
            }
        }

        public IQueryable<Firm> Firms
        {
            get
            {
                return Filter(_context.Firms);
            }
        }

        public IQueryable<FirmAddress> FirmAddresses
        {
            get
            {
                return Filter(_context.FirmAddresses);
            }
        }

        public IQueryable<FirmContact> FirmContacts
        {
            get
            {
                return Filter(_context.FirmContacts);
            }
        }

        public IQueryable<LegalPerson> LegalPersons
        {
            get
            {
                return Filter(_context.LegalPersons);
            }
        }

        public IQueryable<Order> Orders
        {
            get
            {
                return Filter(_context.Orders);
            }
        }

        private IQueryable<T> Filter<T>(IQueryable<T> elements)
            where T : IIdentifiableObject
        {
            return elements.Where(LookupPredicate<T>());
        }

        private Expression<Func<T, bool>> LookupPredicate<T>()
            where T : IIdentifiableObject
        {
            if (typeof(T) == _elementType)
            {
                return x => _ids.Contains(x.Id);
            }
            return _ => true;
        }
    }
}