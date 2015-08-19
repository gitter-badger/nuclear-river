using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public sealed class CustomerIntelligenceTransformationContext : ICustomerIntelligenceContext
    {
        private readonly IErmFactsContext _ermContext;

        public CustomerIntelligenceTransformationContext(IErmFactsContext ermContext)
        {
            if (ermContext == null)
            {
                throw new ArgumentNullException("ermContext");
            }

            _ermContext = ermContext;
        }

        public IQueryable<CategoryGroup> CategoryGroups
        {
            get
            {
                return from categoryGroup in _ermContext.CategoryGroups
                       select new CategoryGroup
                       {
                           Id = categoryGroup.Id,
                           Name = categoryGroup.Name,
                           Rate = categoryGroup.Rate
                       };
            }
        }

        public IQueryable<Client> Clients
        {
            get
            {
                var clientRates = from firm in _ermContext.Firms
                                  join firmAddress in _ermContext.FirmAddresses on firm.Id equals firmAddress.FirmId
                                  join categoryFirmAddress in _ermContext.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                  join categoryOrganizationUnit in _ermContext.CategoryOrganizationUnits on new { categoryFirmAddress.CategoryId, firm.OrganizationUnitId }
                                      equals new { categoryOrganizationUnit.CategoryId, categoryOrganizationUnit.OrganizationUnitId }
                                  join categoryGroup in _ermContext.CategoryGroups on categoryOrganizationUnit.CategoryGroupId equals categoryGroup.Id
                                  group categoryGroup by firm.ClientId into categoryGroups
                                  select new { ClientId = categoryGroups.Key, CategoryGroupId = categoryGroups.Min(x => x.Id) };

                return from client in _ermContext.Clients
                       from rate in clientRates.Where(x => x.ClientId == client.Id).DefaultIfEmpty()
                       select new Client
                              {
                                  Id = client.Id,
                                  Name = client.Name,
                                  CategoryGroupId = rate != null ? rate.CategoryGroupId : 0
                              };
            }
        }

        public IQueryable<ClientContact> ClientContacts
        {
            get
            {
                return from contact in _ermContext.Contacts
                       select new ClientContact
                              {
                                 ClientId = contact.ClientId,
                                  ContactId = contact.Id,
                                  Role = contact.Role,
                                  IsFired = contact.IsFired,
                              };
            }
        }

        public IQueryable<Firm> Firms
        {
            get
            {
                // FIXME {all, 03.04.2015}: the obtained SQL is too complex and slow

                var clientsHavingPhone = from contact in _ermContext.Contacts
                                         where contact.HasPhone
                                         select contact.ClientId;
                var clientsHavingWebsite = from contact in _ermContext.Contacts
                                           where contact.HasWebsite
                                           select contact.ClientId;

                var firmsHavingPhone = from firmContact in _ermContext.FirmContacts.Where(x => x.HasPhone)
                                       join firmAddress in _ermContext.FirmAddresses on firmContact.FirmAddressId equals firmAddress.Id
                                       select firmAddress.FirmId;
                var firmsHavingWebsite = from firmContact in _ermContext.FirmContacts.Where(x => x.HasWebsite)
                                         join firmAddress in _ermContext.FirmAddresses on firmContact.FirmAddressId equals firmAddress.Id
                                         select firmAddress.FirmId;

                // TODO {all, 02.04.2015}: CategoryGroupId processing
                return from firm in _ermContext.Firms
                       join project in _ermContext.Projects on firm.OrganizationUnitId equals project.OrganizationUnitId
                       let firmClient = _ermContext.Clients.SingleOrDefault(client => client.Id == firm.ClientId)
                       let rates = from firmAddress in _ermContext.FirmAddresses.Where(firmAddress => firmAddress.FirmId == firm.Id)
                                   join categoryFirmAddress in _ermContext.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                   join categoryOrganizationUnit in _ermContext.CategoryOrganizationUnits on new { categoryFirmAddress.CategoryId, firm.OrganizationUnitId } equals
                                       new { categoryOrganizationUnit.CategoryId, categoryOrganizationUnit.OrganizationUnitId }
                                   join categoryGroup in _ermContext.CategoryGroups on categoryOrganizationUnit.CategoryGroupId equals categoryGroup.Id
                                   orderby categoryGroup.Rate descending
                                   select categoryGroup.Id
                       select new Firm
                              {
                                  Id = firm.Id,
                                  Name = firm.Name,
                                  CreatedOn = firm.CreatedOn,
                                  LastDisqualifiedOn = (firmClient != null ? firmClient.LastDisqualifiedOn : firm.LastDisqualifiedOn),
                                  HasPhone = firmsHavingPhone.Contains(firm.Id) || (firmClient != null && firmClient.HasPhone) || (firm.ClientId != null && clientsHavingPhone.Contains(firm.ClientId.Value)),
                                  HasWebsite = firmsHavingWebsite.Contains(firm.Id) || (firmClient != null && firmClient.HasWebsite) || (firm.ClientId != null && clientsHavingWebsite.Contains(firm.ClientId.Value)),
                                  AddressCount = _ermContext.FirmAddresses.Count(fa => fa.FirmId == firm.Id),
                                  CategoryGroupId = rates.FirstOrDefault(),
                                  ClientId = firm.ClientId,
                                  ProjectId = project.Id,
                                  OwnerId = firm.OwnerId,
                                  TerritoryId = firm.TerritoryId
                              };
            }
        }

        public IQueryable<FirmActivity> FirmActivities
        {
            get
            {
                var orders = _ermContext.Orders.GroupBy(order => order.FirmId).Select(group => new { FirmId = group.Key, EndDistributionDateFact = group.Max(x => x.EndDistributionDateFact) });
                var firmActivities = _ermContext.Activities.GroupBy(x => x.FirmId).Select(group => new { FirmId = group.Key, LastActivityOn = group.Max(x => x.ModifiedOn) });
                var clientActivities = _ermContext.Activities.GroupBy(x => x.ClientId).Select(group => new { ClientId = group.Key, LastActivityOn = group.Max(x => x.ModifiedOn) });

                return from firm in _ermContext.Firms
                       from order in orders.Where(x => x.FirmId == firm.Id).DefaultIfEmpty()
                       from firmActivity in firmActivities.Where(x => x.FirmId == firm.Id).DefaultIfEmpty()
                       from clientActivity in clientActivities.Where(x => x.ClientId == firm.ClientId).DefaultIfEmpty()
                       select new FirmActivity
                              {
                                  FirmId = firm.Id,
                                  LastDistributedOn = order.EndDistributionDateFact,
                                  LastActivityOn = firmActivity.LastActivityOn > clientActivity.LastActivityOn ? firmActivity.LastActivityOn : clientActivity.LastActivityOn
                              };
            }
        }

        public IQueryable<FirmBalance> FirmBalances
        {
            get
            {
                return from account in _ermContext.Accounts
                       join legalPerson in _ermContext.LegalPersons on account.LegalPersonId equals legalPerson.Id
                       join client in _ermContext.Clients on legalPerson.ClientId equals client.Id
                       join branchOfficeOrganizationUnit in _ermContext.BranchOfficeOrganizationUnits on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                       join firm in _ermContext.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                       where firm.ClientId == client.Id
                       select new FirmBalance { AccountId = account.Id, FirmId = firm.Id, Balance = account.Balance };
            }
        }

        public IQueryable<FirmCategory> FirmCategories
        {
            get
            {
                var categories1 = _ermContext.Categories.Where(x => x.Level == 1);
                var categories2 = _ermContext.Categories.Where(x => x.Level == 2);
                var categories3 = _ermContext.Categories.Where(x => x.Level == 3);

                var level3 = from firmAddress in _ermContext.FirmAddresses
                             join categoryFirmAddress in _ermContext.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             select new FirmCategory
                             {
                                 FirmId = firmAddress.FirmId,
                                 CategoryId = category3.Id
                             };

                var level2 = from firmAddress in _ermContext.FirmAddresses
                             join categoryFirmAddress in _ermContext.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             join category2 in categories2 on category3.ParentId equals category2.Id
                             select new FirmCategory
                             {
                                 FirmId = firmAddress.FirmId,
                                 CategoryId = category2.Id
                             };

                var level1 = from firmAddress in _ermContext.FirmAddresses
                             join categoryFirmAddress in _ermContext.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             join category2 in categories2 on category3.ParentId equals category2.Id
                             join category1 in categories1 on category2.ParentId equals category1.Id
                             select new FirmCategory
                             {
                                 FirmId = firmAddress.FirmId,
                                 CategoryId = category1.Id
                             };

                // perform union using distinct
                return level3.Union(level2).Union(level1);
            }
        }

        public IQueryable<Project> Projects
        {
            get
            {
                return from project in _ermContext.Projects
                       select new Project
                       {
                           Id = project.Id,
                           Name = project.Name
                       };
            }
        }

        public IQueryable<ProjectCategory> ProjectCategories
        {
            get
            {
                return from project in _ermContext.Projects
                       join categoryOrganizationUnit in _ermContext.CategoryOrganizationUnits on project.OrganizationUnitId equals categoryOrganizationUnit.OrganizationUnitId
                       join category in _ermContext.Categories on categoryOrganizationUnit.CategoryId equals category.Id
                       select new ProjectCategory
                              {
                                  ProjectId = project.Id,
                                  CategoryId = categoryOrganizationUnit.CategoryId,
                                  Name = category.Name,
                                  Level = category.Level,
                                  ParentId = category.ParentId,
                              };
            }
        }

        public IQueryable<Territory> Territories
        {
            get
            {
                return from territory in _ermContext.Territories
                       join project in _ermContext.Projects on territory.OrganizationUnitId equals project.OrganizationUnitId
                       select new Territory
                       {
                           Id = territory.Id,
                           Name = territory.Name,
                           ProjectId = project.Id
                       };
            }
        }
    }
}