using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    using Facts = CustomerIntelligence.Model.Facts;

    public static partial class Specs
    {
        public static partial class Facts
        {
            public static partial class Map
            {
                // ReSharper disable once InconsistentNaming
                public static class ToCI
                {
                    public static MapSpecification<IQuery, IQueryable<CategoryGroup>> CategoryGroups(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<CategoryGroup>>(
                            q => from categoryGroup in q.For(API.Specifications.Specs.Find.ByIds<Facts::CategoryGroup>(ids))
                                 select new CategoryGroup
                                        {
                                            Id = categoryGroup.Id,
                                            Name = categoryGroup.Name,
                                            Rate = categoryGroup.Rate
                                        });
                    }

                    public static MapSpecification<IQuery, IQueryable<Client>> Clients(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Client>>(
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

                                return from client in q.For(API.Specifications.Specs.Find.ByIds<Facts::Client>(ids))
                                       from rate in clientRates.Where(x => x.ClientId == client.Id).DefaultIfEmpty()
                                       select new Client
                                              {
                                                  Id = client.Id,
                                                  Name = client.Name,
                                                  CategoryGroupId = rate != null ? rate.CategoryGroupId : 0
                                              };
                            });
                    }

                    public static MapSpecification<IQuery, IQueryable<ClientContact>> ClientContacts(IReadOnlyCollection<long> aggregateIds)
                    {
                        var shouldBeFiltered = aggregateIds == null || !aggregateIds.Any();
                        return new MapSpecification<IQuery, IQueryable<ClientContact>>(
                            q => from contact in q.For<Facts::Contact>()
                                 where shouldBeFiltered || aggregateIds.Contains(contact.ClientId)
                                 select new ClientContact
                                        {
                                            ClientId = contact.ClientId,
                                            ContactId = contact.Id,
                                            Role = contact.Role,
                                            IsFired = contact.IsFired,
                                        });
                    }

                    public static MapSpecification<IQuery, IQueryable<Firm>> Firms(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Firm>>(
                            q =>
                            {
                                // FIXME {all, 03.04.2015}: the obtained SQL is too complex and slow
                                var clientsHavingPhone = from contact in q.For<Facts::Contact>()
                                                         where contact.HasPhone
                                                         select contact.ClientId;
                                var clientsHavingWebsite = from contact in q.For<Facts::Contact>()
                                                           where contact.HasWebsite
                                                           select contact.ClientId;

                                var firmsHavingPhone = from firmContact in q.For<Facts::FirmContact>().Where(x => x.HasPhone)
                                                       join firmAddress in q.For<Facts::FirmAddress>() on firmContact.FirmAddressId equals firmAddress.Id
                                                       select firmAddress.FirmId;
                                var firmsHavingWebsite = from firmContact in q.For<Facts::FirmContact>().Where(x => x.HasWebsite)
                                                         join firmAddress in q.For<Facts::FirmAddress>() on firmContact.FirmAddressId equals firmAddress.Id
                                                         select firmAddress.FirmId;

                                // TODO {all, 02.04.2015}: CategoryGroupId processing
                                return from firm in q.For(API.Specifications.Specs.Find.ByIds<Facts::Firm>(ids))
                                       join project in q.For<Facts::Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                       let firmClient = q.For<Facts::Client>().SingleOrDefault(client => client.Id == firm.ClientId)
                                       let rates = from firmAddress in q.For<Facts::FirmAddress>().Where(firmAddress => firmAddress.FirmId == firm.Id)
                                                   join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                                   join categoryOrganizationUnit in q.For<Facts::CategoryOrganizationUnit>() on new { categoryFirmAddress.CategoryId, firm.OrganizationUnitId } equals
                                                       new { categoryOrganizationUnit.CategoryId, categoryOrganizationUnit.OrganizationUnitId }
                                                   join categoryGroup in q.For<Facts::CategoryGroup>() on categoryOrganizationUnit.CategoryGroupId equals categoryGroup.Id
                                                   orderby categoryGroup.Rate descending
                                                   select categoryGroup.Id
                                       select new Firm
                                       {
                                           Id = firm.Id,
                                           Name = firm.Name,
                                           CreatedOn = firm.CreatedOn,
                                           LastDisqualifiedOn = (firmClient != null ? firmClient.LastDisqualifiedOn : firm.LastDisqualifiedOn),
                                           LastDistributedOn = q.For<Facts::Order>().Where(o => o.FirmId == firm.Id).Select(d => d.EndDistributionDateFact).Cast<DateTimeOffset?>().Max(),
                                           HasPhone = firmsHavingPhone.Contains(firm.Id) || (firmClient != null && firmClient.HasPhone) || (firm.ClientId != null && clientsHavingPhone.Contains(firm.ClientId.Value)),
                                           HasWebsite = firmsHavingWebsite.Contains(firm.Id) || (firmClient != null && firmClient.HasWebsite) || (firm.ClientId != null && clientsHavingWebsite.Contains(firm.ClientId.Value)),
                                           AddressCount = q.For<Facts::FirmAddress>().Count(fa => fa.FirmId == firm.Id),
                                           CategoryGroupId = rates.FirstOrDefault(),
                                           ClientId = firm.ClientId,
                                           ProjectId = project.Id,
                                           OwnerId = firm.OwnerId,
                                           TerritoryId = firm.TerritoryId
                                       };
                            });
                    }

                    public static MapSpecification<IQuery, IQueryable<FirmBalance>> FirmBalances(IReadOnlyCollection<long> aggregateIds)
                    {
                        var shouldBeFiltered = aggregateIds == null || !aggregateIds.Any();
                        return new MapSpecification<IQuery, IQueryable<FirmBalance>>(
                            q => from account in q.For<Facts::Account>()
                                 join legalPerson in q.For<Facts::LegalPerson>() on account.LegalPersonId equals legalPerson.Id
                                 join client in q.For<Facts::Client>() on legalPerson.ClientId equals client.Id
                                 join branchOfficeOrganizationUnit in q.For<Facts::BranchOfficeOrganizationUnit>() on account.BranchOfficeOrganizationUnitId equals
                                     branchOfficeOrganizationUnit.Id
                                 join firm in q.For<Facts::Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 where shouldBeFiltered || aggregateIds.Contains(firm.Id)
                                 where firm.ClientId == client.Id
                                 select new FirmBalance { AccountId = account.Id, FirmId = firm.Id, Balance = account.Balance });
                    }

                    public static MapSpecification<IQuery, IQueryable<FirmCategory>> FirmCategories(IReadOnlyCollection<long> aggregateIds)
                    {
                        var shouldBeFiltered = aggregateIds == null || !aggregateIds.Any();
                        return new MapSpecification<IQuery, IQueryable<FirmCategory>>(
                            q =>
                            {
                                var categories1 = q.For<Facts::Category>().Where(x => x.Level == 1);
                                var categories2 = q.For<Facts::Category>().Where(x => x.Level == 2);
                                var categories3 = q.For<Facts::Category>().Where(x => x.Level == 3);

                                var level3 = from firmAddress in q.For<Facts::FirmAddress>()
                                             join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                             where shouldBeFiltered || aggregateIds.Contains(firmAddress.FirmId)
                                             select new FirmCategory
                                             {
                                                 FirmId = firmAddress.FirmId,
                                                 CategoryId = category3.Id
                                             };

                                var level2 = from firmAddress in q.For<Facts::FirmAddress>()
                                             join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                             join category2 in categories2 on category3.ParentId equals category2.Id
                                             where shouldBeFiltered || aggregateIds.Contains(firmAddress.FirmId)
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
                                             where shouldBeFiltered || aggregateIds.Contains(firmAddress.FirmId)
                                             select new FirmCategory
                                             {
                                                 FirmId = firmAddress.FirmId,
                                                 CategoryId = category1.Id
                                             };

                                // perform union using distinct
                                return level3.Union(level2).Union(level1);
                            });
                    }

                    public static MapSpecification<IQuery, IQueryable<Project>> Projects(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Project>>(
                            q => from project in q.For(API.Specifications.Specs.Find.ByIds<Facts::Project>(ids))
                                 select new Project
                                        {
                                            Id = project.Id,
                                            Name = project.Name
                                        });
                    }

                    public static MapSpecification<IQuery, IQueryable<ProjectCategory>> ProjectCategories(IReadOnlyCollection<long> aggregateIds)
                    {
                        var shouldBeFiltered = aggregateIds == null || !aggregateIds.Any();
                        return new MapSpecification<IQuery, IQueryable<ProjectCategory>>(
                            q => from project in q.For<Facts::Project>()
                                 join categoryOrganizationUnit in q.For<Facts::CategoryOrganizationUnit>() on project.OrganizationUnitId equals categoryOrganizationUnit.OrganizationUnitId
                                 join category in q.For<Facts::Category>() on categoryOrganizationUnit.CategoryId equals category.Id
                                 where shouldBeFiltered || aggregateIds.Contains(project.Id)
                                 select new ProjectCategory
                                 {
                                     ProjectId = project.Id,
                                     CategoryId = categoryOrganizationUnit.CategoryId,
                                     Name = category.Name,
                                     Level = category.Level,
                                     ParentId = category.ParentId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Territory>> Territories(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Territory>>(
                            q => from territory in q.For(API.Specifications.Specs.Find.ByIds<Facts::Territory>(ids))
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
}