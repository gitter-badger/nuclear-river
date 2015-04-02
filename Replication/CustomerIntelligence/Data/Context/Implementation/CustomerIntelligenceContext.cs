﻿using System;
using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public class CustomerIntelligenceContext : ICustomerIntelligenceContext
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

        public IQueryable<Client> Clients
        {
            get
            {
                return _context.GetTable<Client>();
            }
        }

        public IQueryable<Contact> Contacts
        {
            get
            {
                return _context.GetTable<Contact>();
            }
        }

        public IQueryable<Firm> Firms
        {
            get
            {
                return _context.GetTable<Firm>();
            }
        }

        public IQueryable<FirmBalance> FirmBalances
        {
            get
            {
                return _context.GetTable<FirmBalance>();
            }
        }

        public IQueryable<FirmCategory> FirmCategories
        {
            get
            {
                return _context.GetTable<FirmCategory>();
            }
        }

        public IQueryable<FirmCategoryGroup> FirmCategoryGroups
        {
            get
            {
                return _context.GetTable<FirmCategoryGroup>();
            }
        }
    }
}