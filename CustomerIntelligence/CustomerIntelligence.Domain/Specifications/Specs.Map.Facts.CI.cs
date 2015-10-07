using System;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Facts
            {
                // ReSharper disable once InconsistentNaming
                public static class ToCI
                {
                    public readonly static MapSpecification<IQuery, IQueryable<CategoryGroup>> CategoryGroups =
                        new MapSpecification<IQuery, IQueryable<CategoryGroup>>(
                            q => from categoryGroup in q.For<Facts::CategoryGroup>()
                                 select new CategoryGroup
                                        {
                                            Id = categoryGroup.Id,
                                            Name = categoryGroup.Name,
                                            Rate = categoryGroup.Rate
                                        });

                    public readonly static MapSpecification<IQuery, IQueryable<Client>> Clients =
                        new MapSpecification<IQuery, IQueryable<Client>>(
                            q =>
                            {
                                var clientRates = from firm in q.For<Facts::Firm>()
                                                  join firmAddress in q.For<Facts::FirmAddress>() on firm.Id equals firmAddress.FirmId
                                                  join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                                  join categoryOrganizationUnit in q.For<Facts::CategoryOrganizationUnit>() on
                                                      new { categoryFirmAddress.CategoryId, firm.OrganizationUnitId }
                                                      equals new { categoryOrganizationUnit.CategoryId, categoryOrganizationUnit.OrganizationUnitId }
                                                  join categoryGroup in q.For<Facts::CategoryGroup>() on categoryOrganizationUnit.CategoryGroupId equals categoryGroup.Id
                                                  group categoryGroup by firm.ClientId
                                                  into categoryGroups
                                                  select new { ClientId = categoryGroups.Key, CategoryGroupId = categoryGroups.Min(x => x.Id) };
                                return from client in q.For<Facts::Client>()
                                       from rate in clientRates.Where(x => x.ClientId == client.Id).DefaultIfEmpty()
                                       select new Client
                                              {
                                                  Id = client.Id,
                                                  Name = client.Name,
                                                  CategoryGroupId = rate != null ? rate.CategoryGroupId : 0
                                              };
                            });

                    public readonly static MapSpecification<IQuery, IQueryable<ClientContact>> ClientContacts =
                        new MapSpecification<IQuery, IQueryable<ClientContact>>(
                            q => from contact in q.For<Facts::Contact>()
                                 select new ClientContact
                                        {
                                            ClientId = contact.ClientId,
                                            ContactId = contact.Id,
                                            Role = contact.Role,
                                        });

                    public readonly static MapSpecification<IQuery, IQueryable<Firm>> Firms =
                         new MapSpecification<IQuery, IQueryable<Firm>>(
                            q =>
                            {
                                // FIXME {all, 03.04.2015}: the obtained SQL is too complex and slow
                                var clientsHavingPhone = from contact in q.For<Facts::Contact>()
                                                         where contact.HasPhone
                                                         select (long?)contact.ClientId;
                                var clientsHavingWebsite = from contact in q.For<Facts::Contact>()
                                                           where contact.HasWebsite
                                                           select (long?)contact.ClientId;

                                var firmsHavingPhone = from firmContact in q.For<Facts::FirmContact>().Where(x => x.HasPhone)
                                                       join firmAddress in q.For<Facts::FirmAddress>() on firmContact.FirmAddressId equals firmAddress.Id
                                                       select firmAddress.FirmId;
                                var firmsHavingWebsite = from firmContact in q.For<Facts::FirmContact>().Where(x => x.HasWebsite)
                                                         join firmAddress in q.For<Facts::FirmAddress>() on firmContact.FirmAddressId equals firmAddress.Id
                                                         select firmAddress.FirmId;

                                // TODO {all, 02.04.2015}: CategoryGroupId processing
                                return from firm in q.For<Facts::Firm>()
                                       join project in q.For<Facts::Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                       join client in q.For<Facts::Client>() on firm.ClientId equals client.Id into clients
                                       from client in clients.DefaultIfEmpty(new Facts::Client())
                                       let rates = from firmAddress in q.For<Facts::FirmAddress>()
                                                   where firmAddress.FirmId == firm.Id
                                                   join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                                   join categoryOrganizationUnit in q.For<Facts::CategoryOrganizationUnit>() on
                                                       new { categoryFirmAddress.CategoryId, firm.OrganizationUnitId } equals
                                                       new { categoryOrganizationUnit.CategoryId, categoryOrganizationUnit.OrganizationUnitId }
                                                   join categoryGroup in q.For<Facts::CategoryGroup>() on categoryOrganizationUnit.CategoryGroupId equals categoryGroup.Id
                                                   orderby categoryGroup.Rate descending
                                                   select categoryGroup.Id
                                       select new Firm
                                              {
                                                  Id = firm.Id,
                                                  Name = firm.Name,
                                                  CreatedOn = firm.CreatedOn,
                                                  LastDisqualifiedOn = client.LastDisqualifiedOn ?? firm.LastDisqualifiedOn,
                                                  LastDistributedOn = q.For<Facts::Order>()
                                                                       .Where(order => order.FirmId == firm.Id)
                                                                       .Select(order => order.EndDistributionDateFact)
                                                                       .Cast<DateTimeOffset?>()
                                                                       .Max(),
                                                  HasPhone = firmsHavingPhone.Contains(firm.Id) || client.HasPhone || clientsHavingPhone.Contains(firm.ClientId),
                                                  HasWebsite = firmsHavingWebsite.Contains(firm.Id) || client.HasWebsite || clientsHavingWebsite.Contains(firm.ClientId),
                                                  AddressCount = q.For<Facts::FirmAddress>().Count(fa => fa.FirmId == firm.Id),
                                                  CategoryGroupId = rates.FirstOrDefault(),
                                                  ClientId = firm.ClientId,
                                                  ProjectId = project.Id,
                                                  OwnerId = firm.OwnerId
                                              };
                            });

                    public readonly static MapSpecification<IQuery, IQueryable<FirmActivity>> FirmActivities =
                        new MapSpecification<IQuery, IQueryable<FirmActivity>>(
                            q =>
                            {
                                var firmActivities = q.For<Facts::Activity>()
                                                      .Where(x => x.FirmId.HasValue)
                                                      .GroupBy(x => x.FirmId)
                                                      .Select(group => new { FirmId = group.Key, LastActivityOn = group.Max(x => x.ModifiedOn) });
                                var clientActivities = q.For<Facts::Activity>()
                                                        .Where(x => x.ClientId.HasValue)
                                                        .GroupBy(x => x.ClientId)
                                                        .Select(group => new { ClientId = group.Key, LastActivityOn = group.Max(x => x.ModifiedOn) });

                                return from firm in q.For<Facts::Firm>()
                                       from lastFirmActivity in firmActivities.Where(x => x.FirmId == firm.Id).Select(x => (DateTimeOffset?)x.LastActivityOn).DefaultIfEmpty()
                                       from lastClientActivity in
                                           clientActivities.Where(x => x.ClientId == firm.ClientId).Select(x => (DateTimeOffset?)x.LastActivityOn).DefaultIfEmpty()
                                       select new FirmActivity
                                              {
                                                  FirmId = firm.Id,
                                                  LastActivityOn = lastFirmActivity != null && lastClientActivity != null
                                                                       ? (lastFirmActivity < lastClientActivity ? lastClientActivity : lastFirmActivity)
                                                                       : (lastClientActivity ?? lastFirmActivity),
                                              };
                            });

                    public readonly static MapSpecification<IQuery, IQueryable<FirmBalance>> FirmBalances =
                        new MapSpecification<IQuery, IQueryable<FirmBalance>>(
                            q => from account in q.For<Facts::Account>()
                                 join legalPerson in q.For<Facts::LegalPerson>() on account.LegalPersonId equals legalPerson.Id
                                 join client in q.For<Facts::Client>() on legalPerson.ClientId equals client.Id
                                 join branchOfficeOrganizationUnit in q.For<Facts::BranchOfficeOrganizationUnit>() on account.BranchOfficeOrganizationUnitId equals
                                     branchOfficeOrganizationUnit.Id
                                 join firm in q.For<Facts::Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 where firm.ClientId == client.Id
                                 select new FirmBalance { AccountId = account.Id, FirmId = firm.Id, Balance = account.Balance });

                    public readonly static MapSpecification<IQuery, IQueryable<FirmCategory>> FirmCategories = 
                        new MapSpecification<IQuery, IQueryable<FirmCategory>>(
                            q =>
                            {
                                var categories1 = q.For<Facts::Category>().Where(x => x.Level == 1);
                                var categories2 = q.For<Facts::Category>().Where(x => x.Level == 2);
                                var categories3 = q.For<Facts::Category>().Where(x => x.Level == 3);

                                var level3 = from firmAddress in q.For<Facts::FirmAddress>()
                                             join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                             select new FirmCategory
                                             {
                                                 FirmId = firmAddress.FirmId,
                                                 CategoryId = category3.Id
                                             };

                                var level2 = from firmAddress in q.For<Facts::FirmAddress>()
                                             join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                             join category2 in categories2 on category3.ParentId equals category2.Id
                                             select new FirmCategory
                                             {
                                                 FirmId = firmAddress.FirmId,
                                                 CategoryId = category2.Id
                                             };

                                var level1 = from firmAddress in q.For<Facts::FirmAddress>()
                                             join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
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
                            });

                    public static readonly MapSpecification<IQuery, IQueryable<FirmTerritory>> FirmTerritories =
                        new MapSpecification<IQuery, IQueryable<FirmTerritory>>(
                            q => (from firmAddress in q.For<Facts::FirmAddress>()
                                  select new FirmTerritory { FirmId = firmAddress.FirmId, TerritoryId = firmAddress.TerritoryId })
                                     .Distinct());

                    public readonly static MapSpecification<IQuery, IQueryable<Project>> Projects =
                        new MapSpecification<IQuery, IQueryable<Project>>(
                            q => from project in q.For<Facts::Project>()
                                 select new Project
                                        {
                                            Id = project.Id,
                                            Name = project.Name
                                        });

                    public readonly static MapSpecification<IQuery, IQueryable<ProjectCategory>> ProjectCategories =
                        new MapSpecification<IQuery, IQueryable<ProjectCategory>>(
                            q => from project in q.For<Facts::Project>()
                                 join categoryOrganizationUnit in q.For<Facts::CategoryOrganizationUnit>() on project.OrganizationUnitId equals categoryOrganizationUnit.OrganizationUnitId
                                 join category in q.For<Facts::Category>() on categoryOrganizationUnit.CategoryId equals category.Id
                                 select new ProjectCategory
                                 {
                                     ProjectId = project.Id,
                                     CategoryId = categoryOrganizationUnit.CategoryId,
                                     Name = category.Name,
                                     Level = category.Level,
                                     ParentId = category.ParentId,
                                 });

                    public readonly static MapSpecification<IQuery, IQueryable<Territory>> Territories =
                        new MapSpecification<IQuery, IQueryable<Territory>>(
                            q => from territory in q.For<Facts::Territory>()
                                 join project in q.For<Facts::Project>() on territory.OrganizationUnitId equals project.OrganizationUnitId
                                 select new Territory
                                        {
                                            Id = territory.Id,
                                            Name = territory.Name,
                                            ProjectId = project.Id
                                        });
                }
            }
        }
    }
}