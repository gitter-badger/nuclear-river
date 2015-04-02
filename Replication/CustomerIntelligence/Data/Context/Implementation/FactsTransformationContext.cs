using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
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

        public IQueryable<Account> Accounts
        {
            get
            {
                return from account in _ermContext.Accounts
                       select new Account
                              {
                                  Id = account.Id,
                                  Balance = account.Balance,
                                  LegalPersonId = account.LegalPersonId,
                              };
            }
        }

        public IQueryable<CategoryFirmAddress> CategoryFirmAddresses
        {
            get
            {
                return from categoryFirmAddress in _ermContext.CategoryFirmAddresses
                       select new CategoryFirmAddress
                              {
                                  Id = categoryFirmAddress.Id,
                                  CategoryId = categoryFirmAddress.CategoryId,
                                  FirmAddressId = categoryFirmAddress.FirmAddressId,
                              };
            }
        }

        public IQueryable<CategoryOrganizationUnit> CategoryOrganizationUnits
        {
            get
            {
                return from categoryOrganizationUnit in _ermContext.CategoryOrganizationUnits
                       select new CategoryOrganizationUnit
                              {
                                  Id = categoryOrganizationUnit.Id,
                                  CategoryId = categoryOrganizationUnit.CategoryId,
                                  CategoryGroupId = categoryOrganizationUnit.CategoryGroupId,
                                  OrganizationUnitId = categoryOrganizationUnit.OrganizationUnitId,
                              };
            }
        }

        public IQueryable<Client> Clients
        {
            get
            {
                return from client in _ermContext.Clients
                       select new Client
                              {
                                  Id = client.Id,
                                  Name = client.Name,
                                  LastDisqualifiedOn = client.LastDisqualifyTime,
                                  HasPhone = (client.MainPhoneNumber ?? client.AdditionalPhoneNumber1 ?? client.AdditionalPhoneNumber2) != null,
                                  HasWebsite = client.Website != null
                              };
            }
        }

        public IQueryable<Contact> Contacts
        {
            get
            {
                return from contact in _ermContext.Contacts
                       select new Contact
                              {
                                  Id = contact.Id,
                                  Role = ConvertAccountRole(contact.Role),
                                  IsFired = contact.IsFired,
                                  HasPhone = (contact.MainPhoneNumber ?? contact.MobilePhoneNumber ?? contact.HomePhoneNumber ?? contact.AdditionalPhoneNumber) != null,
                                  HasWebsite = contact.Website != null,
                                  ClientId = contact.ClientId
                              };
            }
        }

        public IQueryable<Firm> Firms
        {
            get
            {
                return from firm in _ermContext.Firms
                       select new Firm
                              {
                                  Id = firm.Id,
                                  Name = firm.Name,
                                  CreatedOn = firm.CreatedOn,
                                  LastDisqualifiedOn = firm.LastDisqualifyTime,
                                  ClientId = firm.ClientId,
                                  OrganizationUnitId = firm.OrganizationUnitId,
                                  TerritoryId = firm.TerritoryId
                              };
            }
        }

        public IQueryable<FirmAddress> FirmAddresses
        {
            get
            {
                return from firmAddress in _ermContext.FirmAddresses
                       select new FirmAddress
                              {
                                  Id = firmAddress.Id,
                                  FirmId = firmAddress.FirmId,
                              };
            }
        }

        public IQueryable<FirmContact> FirmContacts
        {
            get
            {
                return from firmContact in _ermContext.FirmContacts
                       select new FirmContact
                              {
                                  Id = firmContact.Id,
                                  ContactType = firmContact.ContactType,
                                  FirmAddressId = (long)firmContact.FirmAddressId,
                              };
            }
        }

        public IQueryable<LegalPerson> LegalPersons
        {
            get
            {
                return from legalPerson in _ermContext.LegalPersons
                       select new LegalPerson
                              {
                                  Id = legalPerson.Id,
                                  ClientId = (long)legalPerson.ClientId,
                              };
            }
        }

        public IQueryable<Order> Orders
        {
            get
            {
                return from order in _ermContext.Orders
                       select new Order
                              {
                                  Id = order.Id,
                                  EndDistributionDateFact = order.EndDistributionDateFact,
                                  FirmId = order.FirmId,
                              };
            }
        }

        private static int ConvertAccountRole(int value)
        {
            switch (value)
            {
                case 200000:
                    return 1;
                case 200001:
                    return 2;
                case 200002:
                    return 3;
                default:
                    return 0;
            }
        }
    }
}