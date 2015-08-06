using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public sealed class ErmFactsTransformationContext : IErmFactsContext
    {
        private readonly IErmContext _context;

        public ErmFactsTransformationContext(IErmContext context)
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
                return from account in _context.Accounts
                       select new Account
                              {
                                  Id = account.Id,
                                  Balance = account.Balance,
                                  BranchOfficeOrganizationUnitId = account.BranchOfficeOrganizationUnitId,
                                  LegalPersonId = account.LegalPersonId,
                              };
            }
        }

        public IQueryable<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits
        {
            get
            {
                return from branchOfficeOrganizationUnit in _context.BranchOfficeOrganizationUnits
                       select new BranchOfficeOrganizationUnit
                       {
                           Id = branchOfficeOrganizationUnit.Id,
                           OrganizationUnitId = branchOfficeOrganizationUnit.OrganizationUnitId
                       };
            }
        }

        public IQueryable<Category> Categories
        {
            get
            {
                return from category in _context.Categories
                       select new Category
                       {
                           Id = category.Id,
                           Name = category.Name,
                           Level = category.Level,
                           ParentId = category.ParentId
                       };
            }
        }

        public IQueryable<CategoryGroup> CategoryGroups
        {
            get
            {
                return from categoryGroup in _context.CategoryGroups
                       select new CategoryGroup
                       {
                           Id = categoryGroup.Id,
                           Name = categoryGroup.Name,
                           Rate = categoryGroup.Rate
                       };
            }
        }

        public IQueryable<CategoryFirmAddress> CategoryFirmAddresses
        {
            get
            {
                return from categoryFirmAddress in _context.CategoryFirmAddresses
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
                return from categoryOrganizationUnit in _context.CategoryOrganizationUnits
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
                return from client in _context.Clients
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
                return from contact in _context.Contacts
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
                return from firm in _context.Firms
                       select new Firm
                              {
                                  Id = firm.Id,
                                  Name = firm.Name,
                                  CreatedOn = firm.CreatedOn,
                                  LastDisqualifiedOn = firm.LastDisqualifyTime,
                                  ClientId = firm.ClientId,
                                  OrganizationUnitId = firm.OrganizationUnitId,
                                  OwnerId = firm.OwnerId,
                                  TerritoryId = firm.TerritoryId
                              };
            }
        }

        public IQueryable<FirmAddress> FirmAddresses
        {
            get
            {
                return from firmAddress in _context.FirmAddresses
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
                return from firmContact in _context.FirmContacts
                       where firmContact.FirmAddressId != null && (firmContact.ContactType == ContactType.Phone || firmContact.ContactType == ContactType.Website)
                       select new FirmContact
                              {
                                  Id = firmContact.Id,
                                  HasPhone = firmContact.ContactType == ContactType.Phone,
                                  HasWebsite = firmContact.ContactType == ContactType.Website,
                                  FirmAddressId = firmContact.FirmAddressId.Value,
                              };
            }
        }

        public IQueryable<LegalPerson> LegalPersons
        {
            get
            {
                return from legalPerson in _context.LegalPersons
                       where legalPerson.ClientId != null
                       select new LegalPerson
                              {
                                  Id = legalPerson.Id,
                                  ClientId = legalPerson.ClientId.Value,
                              };
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

                return from order in _context.Orders
                       where orderStates.Contains(order.WorkflowStepId)
                       select new Order
                              {
                                  Id = order.Id,
                                  EndDistributionDateFact = order.EndDistributionDateFact,
                                  FirmId = order.FirmId,
                              };
            }
        }

        public IQueryable<Project> Projects
        {
            get
            {
                return from project in _context.Projects
                       where project.OrganizationUnitId != null
                       select new Project
                       {
                           Id = project.Id, 
                           Name = project.Name,
                           OrganizationUnitId = project.OrganizationUnitId.Value
                       };
            }
        }

        public IQueryable<Territory> Territories
        {
            get
            {
                return from territory in _context.Territories
                       select new Territory
                       {
                           Id = territory.Id, 
                           Name = territory.Name, 
                           OrganizationUnitId = territory.OrganizationUnitId
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

        private static class ContactType
        {
            public const int Phone = 1;
            public const int Website = 4;
        }

        private static class OrderState
        {
            public const int OnTermination = 4;
            public const int Archive = 6;
        }
    }
}