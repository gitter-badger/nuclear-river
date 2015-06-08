using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public sealed class CustomerIntelligenceTransformationContext : ICustomerIntelligenceContext
    {
        private readonly IErmFactsContext _ermContext;
        private readonly IBitFactsContext _bitContext;

        public CustomerIntelligenceTransformationContext(IErmFactsContext ermContext, IBitFactsContext bitContext)
        {
            if (ermContext == null)
            {
                throw new ArgumentNullException("ermContext");
            }

            if (bitContext == null)
            {
                throw new ArgumentNullException("bitContext");
            }

            _ermContext = ermContext;
            _bitContext = bitContext;
        }

        public IQueryable<Category> Categories
        {
            get
            {
                return from category in _ermContext.Categories
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
                // TODO {all, 02.04.2015}: CategoryGroupId processing
                return from client in _ermContext.Clients
                       select new Client
                              {
                                  Id = client.Id,
                                  Name = client.Name,
                                  //CategoryGroupId = null
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
                                  Role = contact.Role,
                                  IsFired = contact.IsFired,
                                  ClientId = contact.ClientId
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
                       join client in _ermContext.Clients on firm.ClientId equals client.Id into firmClients
                       from firmClient in firmClients.DefaultIfEmpty()
                       select new Firm
                              {
                                  Id = firm.Id,
                                  Name = firm.Name,
                                  CreatedOn = firm.CreatedOn,
                                  LastDisqualifiedOn = (firmClient != null ? firmClient.LastDisqualifiedOn : firm.LastDisqualifiedOn),
                                  LastDistributedOn = _ermContext.Orders.Where(o => o.FirmId == firm.Id).Select(d => d.EndDistributionDateFact).Cast<DateTimeOffset?>().Max(),
                                  HasPhone = firmsHavingPhone.Contains(firm.Id) || (firmClient != null && firmClient.HasPhone) || (firm.ClientId != null && clientsHavingPhone.Contains(firm.ClientId.Value)),
                                  HasWebsite = firmsHavingWebsite.Contains(firm.Id) || (firmClient != null && firmClient.HasWebsite) || (firm.ClientId != null && clientsHavingWebsite.Contains(firm.ClientId.Value)),
                                  AddressCount = _ermContext.FirmAddresses.Count(fa => fa.FirmId == firm.Id),
                                  //CategoryGroupId = null,
                                  ClientId = firm.ClientId,
                                  ProjectId = project.Id,
                                  OwnerId = firm.OwnerId,
                                  TerritoryId = firm.TerritoryId
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
                // "left join FirmStatistics" допустим только при условии, что (FirmId, CategoryId) - primary key в ней, иначе эта операция может дать дубли по fc
                return from firmCategory in level3.Union(level2).Union(level1)
                       from statistics in _bitContext.FirmStatistics.Where(x => x.FirmId == firmCategory.FirmId && x.CategoryId == firmCategory.CategoryId).DefaultIfEmpty()
                       select new FirmCategory
                       {
                           FirmId = firmCategory.FirmId,
                           CategoryId = firmCategory.CategoryId,
                           Hits = statistics != null ? statistics.Hits : 0,
                           Shows = statistics != null ? statistics.Shows : 0,
                       };
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
                var firmCategories = from firm in _ermContext.Firms
                                     join firmAddress in _ermContext.FirmAddresses on firm.Id equals firmAddress.FirmId
                                     join categoryFirmAddress in _ermContext.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                     select new { firm.Id, firm.OrganizationUnitId, categoryFirmAddress.CategoryId };

                return from project in _ermContext.Projects
                       join categoryOrganizationUnit in _ermContext.CategoryOrganizationUnits on project.OrganizationUnitId equals categoryOrganizationUnit.OrganizationUnitId
                       join сategoryStatistics in _bitContext.CategoryStatistics on new { ProjectId = project.Id, categoryOrganizationUnit.CategoryId } equals
                           new { сategoryStatistics.ProjectId, сategoryStatistics.CategoryId } into projectCategoryStatistics
                       let firmCount = firmCategories.Where(x => x.OrganizationUnitId == project.OrganizationUnitId && x.CategoryId == categoryOrganizationUnit.CategoryId).Distinct().Count()
                       select new ProjectCategory
                              {
                                  ProjectId = project.Id,
                                  CategoryId = categoryOrganizationUnit.CategoryId,
                                  FirmCount = firmCount,
                                  AdvertisersShare = firmCount != 0 ? (float)projectCategoryStatistics.Select(x => x.AdvertisersCount).SingleOrDefault() / firmCount : 0
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