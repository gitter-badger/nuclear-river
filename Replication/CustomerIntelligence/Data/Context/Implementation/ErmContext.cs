﻿using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
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

        public IQueryable<Account> Accounts
        {
            get
            {
                return from account in _context.GetTable<Account>()
                       where account.IsActive && !account.IsDeleted
                       select account;
            }
        }

        public IQueryable<CategoryFirmAddress> CategoryFirmAddresses
        {
            get
            {
                return from categoryFirmAddress in _context.GetTable<CategoryFirmAddress>()
                       where categoryFirmAddress.IsActive && !categoryFirmAddress.IsDeleted
                       select categoryFirmAddress;
            }
        }

        public IQueryable<CategoryOrganizationUnit> CategoryOrganizationUnits
        {
            get
            {
                return from categoryOrganizationUnit in _context.GetTable<CategoryOrganizationUnit>()
                       where categoryOrganizationUnit.IsActive && !categoryOrganizationUnit.IsDeleted
                       select categoryOrganizationUnit;
            }
        }

        public IQueryable<Firm> Firms
        {
            get
            {
                return from firm in _context.GetTable<Firm>()
                       where firm.IsActive && !firm.IsDeleted && !firm.ClosedForAscertainment
                       select firm;
            }
        }

        public IQueryable<FirmAddress> FirmAddresses
        {
            get
            {
                return from firmAddress in _context.GetTable<FirmAddress>()
                       where firmAddress.IsActive && !firmAddress.IsDeleted && !firmAddress.ClosedForAscertainment
                       select firmAddress;
            }
        }

        public IQueryable<FirmContact> FirmContacts
        {
            get
            {
                return from firmContact in _context.GetTable<FirmContact>()
                       where firmContact.FirmAddressId != null
                       select firmContact;
            }
        }

        public IQueryable<LegalPerson> LegalPersons
        {
            get
            {
                return from legalPerson in _context.GetTable<LegalPerson>()
                       where legalPerson.IsActive && !legalPerson.IsDeleted && legalPerson.ClientId != null
                       select legalPerson;
            }
        }

        public IQueryable<Client> Clients
        {
            get
            {
                return from client in _context.GetTable<Client>()
                       where client.IsActive && !client.IsDeleted
                       select client;
            }
        }

        public IQueryable<Contact> Contacts
        {
            get
            {
                return from contact in _context.GetTable<Contact>()
                       where contact.IsActive && !contact.IsDeleted
                       select contact;
            }
        }

        public IQueryable<Order> Orders
        {
            get
            {
                var orderStates = new HashSet<int>
                                  {
                                      OrderState.OnTermination,
                                      OrderState.Archive
                                  };

                return from order in _context.GetTable<Order>()
                       where order.IsActive && !order.IsDeleted && orderStates.Contains(order.WorkflowStepId)
                       select order;
            }
        }

        private static class OrderState
        {
            public const int OnTermination = 4;
            public const int Archive = 6;
        }
    }
}