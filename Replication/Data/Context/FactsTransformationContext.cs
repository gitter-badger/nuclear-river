using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;

namespace NuClear.AdvancedSearch.Replication.Data.Context
{
    public sealed class FactsTransformationContext : IFactsContext
    {
        private readonly IErmContext _ermContext;

        public FactsTransformationContext(IErmContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _ermContext = context;
        }

        public IQueryable<Fact.Account> Accounts
        {
            get
            {
                return from account in _ermContext.Accounts
                       select new Fact.Account
                              {
                                  Id = account.Id,
                                  Balance = account.Balance,
                                  LegalPersonId = account.LegalPersonId,
                              };
            }
        }

        public IQueryable<Fact.CategoryFirmAddress> CategoryFirmAddresses
        {
            get
            {
                return from categoryFirmAddress in _ermContext.CategoryFirmAddresses
                       select new Fact.CategoryFirmAddress
                              {
                                  Id = categoryFirmAddress.Id,
                                  CategoryId = categoryFirmAddress.CategoryId,
                                  FirmAddressId = categoryFirmAddress.FirmAddressId,
                              };
            }
        }

        public IQueryable<Fact.CategoryOrganizationUnit> CategoryOrganizationUnits
        {
            get
            {
                return from categoryOrganizationUnit in _ermContext.CategoryOrganizationUnits
                       select new Fact.CategoryOrganizationUnit
                              {
                                  Id = categoryOrganizationUnit.Id,
                                  CategoryId = categoryOrganizationUnit.CategoryId,
                                  CategoryGroupId = categoryOrganizationUnit.CategoryGroupId,
                                  OrganizationUnitId = categoryOrganizationUnit.OrganizationUnitId,
                              };
            }
        }

        public IQueryable<Fact.Client> Clients
        {
            get
            {
                return from client in _ermContext.Clients
                       select new Fact.Client
                              {
                                  Id = client.Id,
                                  Name = client.Name,
                                  HasPhone = (client.MainPhoneNumber ?? client.AdditionalPhoneNumber1 ?? client.AdditionalPhoneNumber2) != null,
                                  HasWebsite = client.Website != null
                              };
            }
        }

        public IQueryable<Fact.Contact> Contacts
        {
            get
            {
                return from contact in _ermContext.Contacts
                       select new Fact.Contact
                              {
                                  Id = contact.Id,
                                  Role = contact.Role,
                                  IsFired = contact.IsFired,
                                  HasPhone = (contact.MainPhoneNumber ?? contact.MobilePhoneNumber ?? contact.HomePhoneNumber ?? contact.AdditionalPhoneNumber) != null,
                                  HasWebsite = contact.Website != null,
                                  ClientId = contact.ClientId
                              };
            }
        }


        public IQueryable<Fact.Firm> Firms
        {
            get
            {
                return from firm in _ermContext.Firms
                       select new Fact.Firm
                       {
                           Id = firm.Id,
                           Name = firm.Name,
//                           Role = contact.Role,
//                           IsFired = contact.IsFired,
//                           HasPhone = (contact.MainPhoneNumber ?? contact.MobilePhoneNumber ?? contact.HomePhoneNumber ?? contact.AdditionalPhoneNumber) != null,
//                           HasWebsite = contact.Website != null,
//                           ClientId = contact.ClientId
                       };
            }
        }

        public IQueryable<Fact.FirmAddress> FirmAddresses
        {
            get
            {
                return from firmAddress in _ermContext.FirmAddresses
                       select new Fact.FirmAddress
                       {
                           Id = firmAddress.Id,
                           FirmId = firmAddress.FirmId,
                       };
            }
        }

        public IQueryable<Fact.FirmContact> FirmContacts
        {
            get
            {
                return from firmContact in _ermContext.FirmContacts
                       select new Fact.FirmContact
                       {
                           Id = firmContact.Id,
                           ContactType = firmContact.ContactType,
                           FirmAddressId = firmContact.FirmAddressId,
                       };
            }
        }

        public IQueryable<Fact.LegalPerson> LegalPersons
        {
            get
            {
                return from legalPerson in _ermContext.LegalPersons
                       select new Fact.LegalPerson
                       {
                           Id = legalPerson.Id,
                           ClientId = legalPerson.ClientId,
                       };
            }
        }

        public IQueryable<Fact.Order> Orders
        {
            get
            {
                return from order in _ermContext.Orders
                       select new Fact.Order
                       {
                           Id = order.Id,
                           EndDistributionDateFact = order.EndDistributionDateFact,
                           WorkflowStepId = order.WorkflowStepId,
                           FirmId = order.FirmId,
                       };
            }
        }

    }
}