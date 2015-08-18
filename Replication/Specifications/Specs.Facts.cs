using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Facts
        {
            public static partial class Map
            {
                public static class ToClientAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirm(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firm in q.For(API.Specifications.Specs.Find.ByIds<Firm>(ids))
                                 where firm.ClientId != null
                                 select firm.ClientId.Value);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByContacts(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from client in q.For<Client>()
                                    join contact in q.For(API.Specifications.Specs.Find.ByIds<Contact>(ids)) on client.Id equals contact.ClientId
                                    select client.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryFirmAddress(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryFirmAddress in q.For(API.Specifications.Specs.Find.ByIds<CategoryFirmAddress>(ids))
                                 join firmAddress in q.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                 join firm in q.For<Firm>() on firmAddress.FirmId equals firm.Id
                                 where firm.ClientId.HasValue
                                 select firm.ClientId.Value);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryGroup(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from categoryGroup in q.For(API.Specifications.Specs.Find.ByIds<CategoryGroup>(ids))
                                  join categoryOrganizationUnit in q.For<CategoryOrganizationUnit>() on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                                  join categoryFirmAddress in q.For<CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                                  join firmAddress in q.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                  join firm in q.For<Firm>()
                                      on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals new { firm.OrganizationUnitId, FirmId = firm.Id }
                                  where firm.ClientId.HasValue
                                  select firm.ClientId.Value).Distinct());
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryOrganizationUnit(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryOrganizationUnit in q.For(API.Specifications.Specs.Find.ByIds<CategoryOrganizationUnit>(ids))
                                 join categoryFirmAddress in q.For<CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                                 join firmAddress in q.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                 join firm in q.For<Firm>()
                                     on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals new { firm.OrganizationUnitId, FirmId = firm.Id }
                                 where firm.ClientId.HasValue
                                 select firm.ClientId.Value);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmAddress(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firmAddress in q.For(API.Specifications.Specs.Find.ByIds<FirmAddress>(ids))
                                 join firm in q.For<Firm>() on firmAddress.FirmId equals firm.Id
                                 where firm.ClientId.HasValue
                                 select firm.ClientId.Value);
                    }
                }

                public static class ToFirmAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByAccount(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from account in q.For(API.Specifications.Specs.Find.ByIds<Account>(ids))
                                 join legalPerson in q.For<LegalPerson>() on account.LegalPersonId equals legalPerson.Id
                                 join client in q.For<Client>() on legalPerson.ClientId equals client.Id
                                 join branchOfficeOrganizationUnit in q.For<BranchOfficeOrganizationUnit>()
                                     on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                                 join firm in q.For<Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 where firm.ClientId == client.Id
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByBranchOfficeOrganizationUnit(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from branchOfficeOrganizationUnit in q.For(API.Specifications.Specs.Find.ByIds<BranchOfficeOrganizationUnit>(ids))
                                 join firm in q.For<Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategory(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var categories1 = q.For(new FindSpecification<Category>(x => x.Level == 1));
                                var categories2 = q.For(new FindSpecification<Category>(x => x.Level == 2));
                                var categories3 = q.For(new FindSpecification<Category>(x => x.Level == 3));

                                var level3 = from firmAddress in q.For<FirmAddress>()
                                             join categoryFirmAddress in q.For<CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                             where ids.Contains(category3.Id)
                                             select firmAddress.FirmId;

                                var level2 = from firmAddress in q.For<FirmAddress>()
                                             join categoryFirmAddress in q.For<CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                             join category2 in categories2 on category3.ParentId equals category2.Id
                                             where ids.Contains(category2.Id)
                                             select firmAddress.FirmId;

                                var level1 = from firmAddress in q.For<FirmAddress>()
                                             join categoryFirmAddress in q.For<CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                             join category2 in categories2 on category3.ParentId equals category2.Id
                                             join category1 in categories1 on category2.ParentId equals category1.Id
                                             where ids.Contains(category1.Id)
                                             select firmAddress.FirmId;

                                return level3.Union(level2).Union(level1);
                            });
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryFirmAddress(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryFirmAddress in q.For(API.Specifications.Specs.Find.ByIds<CategoryFirmAddress>(ids))
                                 join firmAddress in q.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                 select firmAddress.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryFirmAddressForStatistics(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var changeKeys = from firm in q.For<Firm>()
                                                 join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                                 join firmAddressCategory in q.For(API.Specifications.Specs.Find.ByIds<CategoryFirmAddress>(ids)) on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                                 select new { firm.OrganizationUnitId, firmAddressCategory.CategoryId };

                                var query = from firm in q.For<Firm>()
                                            join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                            join firmAddressCategory in q.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                            join key in changeKeys on new { firm.OrganizationUnitId, firmAddressCategory.CategoryId } equals
                                                new { key.OrganizationUnitId, key.CategoryId }
                                            select firm.Id;

                                return query.Distinct();
                            });
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryOrganizationUnit(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from firm in q.For<Firm>()
                                  join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                  join categoryFirmAddress in q.For<CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                  join categoryOrganizationUnit in q.For(API.Specifications.Specs.Find.ByIds<CategoryOrganizationUnit>(ids))
                                  on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId
                                  where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                                  select firmAddress.FirmId).Distinct());
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByClient(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firm in q.For(new FindSpecification<Firm>(x => ids.Contains(x.ClientId.Value)))
                                 where firm.ClientId != null
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByContacts(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firm in q.For<Firm>()
                                 join client in q.For<Client>() on firm.ClientId equals client.Id
                                 join contact in q.For(API.Specifications.Specs.Find.ByIds<Contact>(ids)) on client.Id equals contact.ClientId
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmForStatistics(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var changeKeys = from firm in q.For(API.Specifications.Specs.Find.ByIds<Firm>(ids))
                                                 join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                                 join firmAddressCategory in q.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                                 select new { firm.OrganizationUnitId, firmAddressCategory.CategoryId };

                                var query = from firm in q.For<Firm>()
                                            join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                            join firmAddressCategory in q.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                            join key in changeKeys on new { firm.OrganizationUnitId, firmAddressCategory.CategoryId } equals
                                                new { key.OrganizationUnitId, key.CategoryId }
                                            select firm.Id;

                                return query.Distinct();
                            });
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryGroup(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from categoryGroup in q.For(API.Specifications.Specs.Find.ByIds<CategoryGroup>(ids))
                                  join categoryOrganizationUnit in q.For<CategoryOrganizationUnit>() on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                                  join categoryFirmAddress in q.For<CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                                  join firmAddress in q.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                  select firmAddress.FirmId).Distinct());
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmAddress(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firmAddress in q.For(API.Specifications.Specs.Find.ByIds<FirmAddress>(ids))
                                 select firmAddress.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmAddressForStatistics(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var changeKeys = from firm in q.For<Firm>()
                                                 join firmAddress in q.For(API.Specifications.Specs.Find.ByIds<FirmAddress>(ids)) on firm.Id equals firmAddress.FirmId
                                                 join firmAddressCategory in q.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                                 select new { firm.OrganizationUnitId, firmAddressCategory.CategoryId };

                                var query = from firm in q.For<Firm>()
                                            join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                            join firmAddressCategory in q.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                            join key in changeKeys
                                                on new { firm.OrganizationUnitId, firmAddressCategory.CategoryId } equals new { key.OrganizationUnitId, key.CategoryId }
                                            select firm.Id;

                                return query.Distinct(); 
                            });
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmContacts(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firmAddress in q.For<FirmAddress>()
                                 join firmContact in q.For(API.Specifications.Specs.Find.ByIds<FirmContact>(ids)) on firmAddress.Id equals firmContact.FirmAddressId
                                 select firmAddress.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByLegalPerson(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from account in q.For<Account>()
                                 join legalPerson in q.For(API.Specifications.Specs.Find.ByIds<LegalPerson>(ids)) on account.LegalPersonId equals legalPerson.Id
                                 join client in q.For<Client>() on legalPerson.ClientId equals client.Id
                                 join branchOfficeOrganizationUnit in q.For<BranchOfficeOrganizationUnit>()
                                     on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                                 join firm in q.For<Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 where firm.ClientId == client.Id
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByOrder(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from order in q.For(API.Specifications.Specs.Find.ByIds<Order>(ids))
                                 select order.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByProject(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from project in q.For(API.Specifications.Specs.Find.ByIds<Project>(ids))
                                 join firm in q.For<Firm>() on project.OrganizationUnitId equals firm.OrganizationUnitId
                                 select firm.Id);
                    }
                }

                public static class ToProjectAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryOrganizationUnit(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryOrganizationUnit in q.For(API.Specifications.Specs.Find.ByIds<CategoryOrganizationUnit>(ids))
                                 join project in q.For<Project>() on categoryOrganizationUnit.OrganizationUnitId equals project.OrganizationUnitId
                                 select project.Id);
                    }
                }

                public static class ToTerritoryAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByProject(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from project in q.For(API.Specifications.Specs.Find.ByIds<Project>(ids))
                                 join territory in q.For<Territory>() on project.OrganizationUnitId equals territory.OrganizationUnitId
                                 select territory.Id);
                    }
                }

                public static class ToStatistics
                {
                    public static MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>> ByFirm(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>>(
                            q => from firm in q.For(API.Specifications.Specs.Find.ByIds<Firm>(ids))
                                 join project in q.For<Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                 join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                 join firmAddressCategory in q.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                 select new CalculateStatisticsOperation { CategoryId = firmAddressCategory.CategoryId, ProjectId = project.Id });
                    }

                    public static MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>> ByFirmAddress(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>>(
                            q => from firm in q.For<Firm>()
                                 join project in q.For<Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                 join firmAddress in q.For(API.Specifications.Specs.Find.ByIds<FirmAddress>(ids)) on firm.Id equals firmAddress.FirmId
                                 join firmAddressCategory in q.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                 select new CalculateStatisticsOperation { CategoryId = firmAddressCategory.CategoryId, ProjectId = project.Id });
                    }

                    public static MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>> ByFirmAddressCategory(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>>(
                            q => from firm in q.For<Firm>()
                                 join project in q.For<Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                 join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                 join firmAddressCategory in q.For(API.Specifications.Specs.Find.ByIds<CategoryFirmAddress>(ids)) on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                 select new CalculateStatisticsOperation { CategoryId = firmAddressCategory.CategoryId, ProjectId = project.Id });
                    }

                    public static MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>> ByProject(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>>(
                            q => from projectId in ids
                                 select new CalculateStatisticsOperation { CategoryId = null, ProjectId = projectId });
                    }
                }
            }
        }
    }
}