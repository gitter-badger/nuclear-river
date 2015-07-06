using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    using Facts = Model.Facts;

    public sealed class CustomerIntelligenceTransformationContext : ICustomerIntelligenceContext
    {
        private readonly IQuery _query;

        public CustomerIntelligenceTransformationContext(IQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            _query = query;
        }

        public IQueryable<CategoryGroup> CategoryGroups
        {
            get
            {
                return from categoryGroup in _query.For<Facts.CategoryGroup>()
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
                var clientRates = from firm in _query.For<Facts.Firm>()
                                  join firmAddress in _query.For<Facts.FirmAddress>() on firm.Id equals firmAddress.FirmId
                                  join categoryFirmAddress in _query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                  join categoryOrganizationUnit in _query.For<Facts.CategoryOrganizationUnit>() on new { categoryFirmAddress.CategoryId, firm.OrganizationUnitId }
                                       equals new { categoryOrganizationUnit.CategoryId, categoryOrganizationUnit.OrganizationUnitId }
                                  join categoryGroup in _query.For<Facts.CategoryGroup>() on categoryOrganizationUnit.CategoryGroupId equals categoryGroup.Id
                                  group categoryGroup by firm.ClientId into categoryGroups
                                  select new { ClientId = categoryGroups.Key, CategoryGroupId = categoryGroups.Min(x => x.Id) };

                return from client in _query.For<Facts.Client>()
                       from rate in clientRates.Where(x => x.ClientId == client.Id).DefaultIfEmpty()
                       select new Client
                              {
                                  Id = client.Id,
                                  Name = client.Name,
                                  CategoryGroupId = rate != null ? rate.CategoryGroupId : 0
                              };
            }
        }

        public IQueryable<Contact> Contacts
        {
            get
            {
                return from contact in _query.For<Facts.Contact>()
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

                var clientsHavingPhone = from contact in _query.For<Facts.Contact>()
                                         where contact.HasPhone
                                         select contact.ClientId;
                var clientsHavingWebsite = from contact in _query.For<Facts.Contact>()
                                           where contact.HasWebsite
                                           select contact.ClientId;

                var firmsHavingPhone = from firmContact in _query.For<Facts.FirmContact>().Where(x => x.HasPhone)
                                       join firmAddress in _query.For<Facts.FirmAddress>() on firmContact.FirmAddressId equals firmAddress.Id
                                       select firmAddress.FirmId;
                var firmsHavingWebsite = from firmContact in _query.For<Facts.FirmContact>().Where(x => x.HasWebsite)
                                         join firmAddress in _query.For<Facts.FirmAddress>() on firmContact.FirmAddressId equals firmAddress.Id
                                         select firmAddress.FirmId;

                // TODO {all, 02.04.2015}: CategoryGroupId processing
                return from firm in _query.For<Facts.Firm>()
                       join project in _query.For<Facts.Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                       let firmClient = _query.For<Facts.Client>().SingleOrDefault(client => client.Id == firm.ClientId)
                       let rates = from firmAddress in _query.For<Facts.FirmAddress>().Where(firmAddress => firmAddress.FirmId == firm.Id)
                                   join categoryFirmAddress in _query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                   join categoryOrganizationUnit in _query.For<Facts.CategoryOrganizationUnit>() on new { categoryFirmAddress.CategoryId, firm.OrganizationUnitId } equals
                                       new { categoryOrganizationUnit.CategoryId, categoryOrganizationUnit.OrganizationUnitId }
                                   join categoryGroup in _query.For<Facts.CategoryGroup>() on categoryOrganizationUnit.CategoryGroupId equals categoryGroup.Id
                                   orderby categoryGroup.Rate descending
                                   select categoryGroup.Id
                       select new Firm
                              {
                                  Id = firm.Id,
                                  Name = firm.Name,
                                  CreatedOn = firm.CreatedOn,
                                  LastDisqualifiedOn = (firmClient != null ? firmClient.LastDisqualifiedOn : firm.LastDisqualifiedOn),
                                  LastDistributedOn = _query.For<Facts.Order>().Where(o => o.FirmId == firm.Id).Select(d => d.EndDistributionDateFact).Cast<DateTimeOffset?>().Max(),
                                  HasPhone = firmsHavingPhone.Contains(firm.Id) || (firmClient != null && firmClient.HasPhone) || (firm.ClientId != null && clientsHavingPhone.Contains(firm.ClientId.Value)),
                                  HasWebsite = firmsHavingWebsite.Contains(firm.Id) || (firmClient != null && firmClient.HasWebsite) || (firm.ClientId != null && clientsHavingWebsite.Contains(firm.ClientId.Value)),
                                  AddressCount = _query.For<Facts.FirmAddress>().Count(fa => fa.FirmId == firm.Id),
                                  CategoryGroupId = rates.FirstOrDefault(),
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
                return from account in _query.For<Facts.Account>()
                       join legalPerson in _query.For<Facts.LegalPerson>() on account.LegalPersonId equals legalPerson.Id
                       join client in _query.For<Facts.Client>() on legalPerson.ClientId equals client.Id
                       join branchOfficeOrganizationUnit in _query.For<Facts.BranchOfficeOrganizationUnit>() on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                       join firm in _query.For<Facts.Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                       where firm.ClientId == client.Id
                       select new FirmBalance { AccountId = account.Id, FirmId = firm.Id, Balance = account.Balance };
            }
        }

        public IQueryable<FirmCategoryPartFirm> FirmCategoriesPartFirm
        {
            get
            {
                var categories1 = _query.For<Facts.Category>().Where(x => x.Level == 1);
                var categories2 = _query.For<Facts.Category>().Where(x => x.Level == 2);
                var categories3 = _query.For<Facts.Category>().Where(x => x.Level == 3);

                var level3 = from firmAddress in _query.For<Facts.FirmAddress>()
                             join categoryFirmAddress in _query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             select new FirmCategoryPartFirm
                             {
                                 FirmId = firmAddress.FirmId,
                                 CategoryId = category3.Id
                             };

                var level2 = from firmAddress in _query.For<Facts.FirmAddress>()
                             join categoryFirmAddress in _query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             join category2 in categories2 on category3.ParentId equals category2.Id
                             select new FirmCategoryPartFirm
                             {
                                 FirmId = firmAddress.FirmId,
                                 CategoryId = category2.Id
                             };

                var level1 = from firmAddress in _query.For<Facts.FirmAddress>()
                             join categoryFirmAddress in _query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             join category2 in categories2 on category3.ParentId equals category2.Id
                             join category1 in categories1 on category2.ParentId equals category1.Id
                             select new FirmCategoryPartFirm
                             {
                                 FirmId = firmAddress.FirmId,
                                 CategoryId = category1.Id
                             };

                // perform union using distinct
                // "left join FirmStatistics" допустим только при условии, что (FirmId, CategoryId) - primary key в ней, иначе эта операция может дать дубли по fc
                return from firmCategory in level3.Union(level2).Union(level1)
                       from statistics in _query.For<Facts.FirmCategoryStatistics>().Where(x => x.FirmId == firmCategory.FirmId && x.CategoryId == firmCategory.CategoryId).DefaultIfEmpty()
                       select new FirmCategoryPartFirm
                       {
                           FirmId = firmCategory.FirmId,
                           CategoryId = firmCategory.CategoryId,
                           Hits = statistics != null ? statistics.Hits : 0,
                           Shows = statistics != null ? statistics.Shows : 0,
                       };
            }
        }

        public IQueryable<FirmCategoryPartProject> FirmCategoriesPartProject
        {
            get
            {
                var firmCategories = (from project in _query.For<Facts.Project>()
                                      join firm in _query.For<Facts.Firm>() on project.OrganizationUnitId equals firm.OrganizationUnitId
                                      join firmAddress in _query.For<Facts.FirmAddress>() on firm.Id equals firmAddress.FirmId
                                      join categoryFirmAddress in _query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                      select new { ProjectId = project.Id, FirmId = firm.Id, categoryFirmAddress.CategoryId }).Distinct();

                var firmCounts = firmCategories.GroupBy(x => new { x.ProjectId, x.CategoryId })
                                         .Select(x => new { x.Key.CategoryId, x.Key.ProjectId, FirmCount = x.Count() });

                return from firmCategory in firmCategories
                       let count = firmCounts.Where(x => x.ProjectId == firmCategory.ProjectId && x.CategoryId == firmCategory.CategoryId).Select(x => x.FirmCount).SingleOrDefault()
                       let statistics = _query.For<Facts.ProjectCategoryStatistics>().Where(x => x.ProjectId == firmCategory.ProjectId && x.CategoryId == firmCategory.CategoryId).Select(x => x.AdvertisersCount).SingleOrDefault()
                       select new FirmCategoryPartProject
                              {
                                  FirmId = firmCategory.FirmId,
                                  CategoryId = firmCategory.CategoryId,
                                  FirmCount = count,
                                  AdvertisersShare = (float)statistics / count
                              };
            }
        }

        public IQueryable<Project> Projects
        {
            get
            {
                return from project in _query.For<Facts.Project>()
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
                return from project in _query.For<Facts.Project>()
                       join categoryOrganizationUnit in _query.For<Facts.CategoryOrganizationUnit>() on project.OrganizationUnitId equals categoryOrganizationUnit.OrganizationUnitId
                       join category in _query.For<Facts.Category>() on categoryOrganizationUnit.CategoryId equals category.Id
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
                return from territory in _query.For<Facts.Territory>()
                       join project in _query.For<Facts.Project>() on territory.OrganizationUnitId equals project.OrganizationUnitId
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