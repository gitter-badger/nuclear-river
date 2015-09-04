using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    using Facts = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

    public sealed class Identity<T>
    {
        public long Id { get; set; }
    }

    public static partial class Specs
    {
        public static partial class Facts
        {
            public static partial class Map
            {
                public static class ToClientAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirm(FindSpecification<Facts::Firm> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firm in q.For(specification)
                                 where firm.ClientId != null
                                 select firm.ClientId.Value);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByContacts(FindSpecification<Facts::Contact> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from client in q.For<Facts::Client>()
                                    join contact in q.For(specification) on client.Id equals contact.ClientId
                                    select client.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryFirmAddress(FindSpecification<Facts::CategoryFirmAddress> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryFirmAddress in q.For(specification)
                                 join firmAddress in q.For<Facts::FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                 join firm in q.For<Facts::Firm>() on firmAddress.FirmId equals firm.Id
                                 where firm.ClientId.HasValue
                                 select firm.ClientId.Value);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryGroup(FindSpecification<Facts::CategoryGroup> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from categoryGroup in q.For(specification)
                                  join categoryOrganizationUnit in q.For<Facts::CategoryOrganizationUnit>() on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                                  join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                                  join firmAddress in q.For<Facts::FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                  join firm in q.For<Facts::Firm>()
                                      on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals new { firm.OrganizationUnitId, FirmId = firm.Id }
                                  where firm.ClientId.HasValue
                                  select firm.ClientId.Value).Distinct());
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryOrganizationUnit(FindSpecification<Facts::CategoryOrganizationUnit> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryOrganizationUnit in q.For(specification)
                                 join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                                 join firmAddress in q.For<Facts::FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                 join firm in q.For<Facts::Firm>()
                                     on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals new { firm.OrganizationUnitId, FirmId = firm.Id }
                                 where firm.ClientId.HasValue
                                 select firm.ClientId.Value);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmAddress(FindSpecification<Facts::FirmAddress> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firmAddress in q.For(specification)
                                 join firm in q.For<Facts::Firm>() on firmAddress.FirmId equals firm.Id
                                 where firm.ClientId.HasValue
                                 select firm.ClientId.Value);
                    }
                }

                public static class ToFirmAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByAccount(FindSpecification<Facts::Account> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from account in q.For(specification)
                                 join legalPerson in q.For<Facts::LegalPerson>() on account.LegalPersonId equals legalPerson.Id
                                 join client in q.For<Facts::Client>() on legalPerson.ClientId equals client.Id
                                 join branchOfficeOrganizationUnit in q.For<Facts::BranchOfficeOrganizationUnit>()
                                     on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                                 join firm in q.For<Facts::Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 where firm.ClientId == client.Id
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByBranchOfficeOrganizationUnit(FindSpecification<Facts::BranchOfficeOrganizationUnit> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from branchOfficeOrganizationUnit in q.For(specification)
                                 join firm in q.For<Facts::Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategory(FindSpecification<Facts::Category> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var categories1 = q.For(new FindSpecification<Facts::Category>(x => x.Level == 1));
                                var categories2 = q.For(new FindSpecification<Facts::Category>(x => x.Level == 2));
                                var categories3 = q.For(new FindSpecification<Facts::Category>(x => x.Level == 3));

                                var level3 = from firmAddress in q.For<Facts::FirmAddress>()
                                             join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                             join category3 in categories3.Where(specification) on categoryFirmAddress.CategoryId equals category3.Id
                                             select firmAddress.FirmId;

                                var level2 = from firmAddress in q.For<Facts::FirmAddress>()
                                             join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                             join category2 in categories2.Where(specification) on category3.ParentId equals category2.Id
                                             select firmAddress.FirmId;

                                var level1 = from firmAddress in q.For<Facts::FirmAddress>()
                                             join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                             join category2 in categories2 on category3.ParentId equals category2.Id
                                             join category1 in categories1.Where(specification) on category2.ParentId equals category1.Id
                                             select firmAddress.FirmId;

                                return level3.Union(level2).Union(level1);
                            });
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryFirmAddress(FindSpecification<Facts::CategoryFirmAddress> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryFirmAddress in q.For(specification)
                                 join firmAddress in q.For<Facts::FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                 select firmAddress.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryFirmAddressForStatistics(FindSpecification<Facts::CategoryFirmAddress> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var changeKeys = from firm in q.For<Facts::Firm>()
                                                 join firmAddress in q.For<Facts::FirmAddress>() on firm.Id equals firmAddress.FirmId
                                                 join firmAddressCategory in q.For(specification) on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                                 select new { firm.OrganizationUnitId, firmAddressCategory.CategoryId };

                                var query = from firm in q.For<Facts::Firm>()
                                            join firmAddress in q.For<Facts::FirmAddress>() on firm.Id equals firmAddress.FirmId
                                            join firmAddressCategory in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                            join key in changeKeys on new { firm.OrganizationUnitId, firmAddressCategory.CategoryId } equals
                                                new { key.OrganizationUnitId, key.CategoryId }
                                            select firm.Id;

                                return query.Distinct();
                            });
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryOrganizationUnit(FindSpecification<Facts::CategoryOrganizationUnit> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from firm in q.For<Facts::Firm>()
                                  join firmAddress in q.For<Facts::FirmAddress>() on firm.Id equals firmAddress.FirmId
                                  join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                  join categoryOrganizationUnit in q.For(specification)
                                  on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId
                                  where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                                  select firmAddress.FirmId).Distinct());
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByClient(FindSpecification<Facts::Client> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firm in q.For<Facts::Firm>()
                                 join client in q.For(specification) on firm.ClientId equals client.Id
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByContacts(FindSpecification<Facts::Contact> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firm in q.For<Facts::Firm>()
                                 join client in q.For<Facts::Client>() on firm.ClientId equals client.Id
                                 join contact in q.For(specification) on client.Id equals contact.ClientId
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmForStatistics(FindSpecification<Facts::Firm> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var changeKeys = from firm in q.For(specification)
                                                 join firmAddress in q.For<Facts::FirmAddress>() on firm.Id equals firmAddress.FirmId
                                                 join firmAddressCategory in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                                 select new { firm.OrganizationUnitId, firmAddressCategory.CategoryId };

                                var query = from firm in q.For<Facts::Firm>()
                                            join firmAddress in q.For<Facts::FirmAddress>() on firm.Id equals firmAddress.FirmId
                                            join firmAddressCategory in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                            join key in changeKeys on new { firm.OrganizationUnitId, firmAddressCategory.CategoryId } equals
                                                new { key.OrganizationUnitId, key.CategoryId }
                                            select firm.Id;

                                return query.Distinct();
                            });
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryGroup(FindSpecification<Facts::CategoryGroup> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from categoryGroup in q.For(specification)
                                  join categoryOrganizationUnit in q.For<Facts::CategoryOrganizationUnit>() on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                                  join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                                  join firmAddress in q.For<Facts::FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                  select firmAddress.FirmId).Distinct());
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmAddress(FindSpecification<Facts::FirmAddress> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firmAddress in q.For(specification)
                                 select firmAddress.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmAddressForStatistics(FindSpecification<Facts::FirmAddress> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var changeKeys = from firm in q.For<Facts::Firm>()
                                                 join firmAddress in q.For(specification) on firm.Id equals firmAddress.FirmId
                                                 join firmAddressCategory in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                                 select new { firm.OrganizationUnitId, firmAddressCategory.CategoryId };

                                var query = from firm in q.For<Facts::Firm>()
                                            join firmAddress in q.For<Facts::FirmAddress>() on firm.Id equals firmAddress.FirmId
                                            join firmAddressCategory in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                            join key in changeKeys
                                                on new { firm.OrganizationUnitId, firmAddressCategory.CategoryId } equals new { key.OrganizationUnitId, key.CategoryId }
                                            select firm.Id;

                                return query.Distinct(); 
                            });
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmContacts(FindSpecification<Facts::FirmContact> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firmAddress in q.For<Facts::FirmAddress>()
                                 join firmContact in q.For(specification) on firmAddress.Id equals firmContact.FirmAddressId
                                 select firmAddress.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByLegalPerson(FindSpecification<Facts::LegalPerson> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from account in q.For<Facts::Account>()
                                 join legalPerson in q.For(specification) on account.LegalPersonId equals legalPerson.Id
                                 join client in q.For<Facts::Client>() on legalPerson.ClientId equals client.Id
                                 join branchOfficeOrganizationUnit in q.For<Facts::BranchOfficeOrganizationUnit>()
                                     on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                                 join firm in q.For<Facts::Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 where firm.ClientId == client.Id
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByOrder(FindSpecification<Facts::Order> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from order in q.For(specification)
                                 select order.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByProject(FindSpecification<Facts::Project> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from project in q.For(specification)
                                 join firm in q.For<Facts::Firm>() on project.OrganizationUnitId equals firm.OrganizationUnitId
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByActivity(FindSpecification<Facts::Activity> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from activity in q.For(specification)
                                 where activity.FirmId.HasValue
                                 select activity.FirmId.Value);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByClientActivity(FindSpecification<Facts::Activity> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from activity in q.For(specification)
                                 join firm in q.For<Facts::Firm>() on activity.ClientId equals firm.ClientId
                                 where activity.ClientId.HasValue
                                 select firm.Id);
                    }
                }

                public static class ToProjectAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryOrganizationUnit(FindSpecification<Facts::CategoryOrganizationUnit> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryOrganizationUnit in q.For(specification)
                                 join project in q.For<Facts::Project>() on categoryOrganizationUnit.OrganizationUnitId equals project.OrganizationUnitId
                                 select project.Id);
                    }
                }

                public static class ToTerritoryAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByProject(FindSpecification<Facts::Project> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from project in q.For(specification)
                                 join territory in q.For<Facts::Territory>() on project.OrganizationUnitId equals territory.OrganizationUnitId
                                 select territory.Id);
                    }
                }

                public static class ToStatistics
                {
                    public static MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>> ByFirm(FindSpecification<Facts::Firm> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>(
                            q => from firm in q.For(specification)
                                 join project in q.For<Facts::Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                 join firmAddress in q.For<Facts::FirmAddress>() on firm.Id equals firmAddress.FirmId
                                 join firmAddressCategory in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                 select new Tuple<long, long?>(project.Id, firmAddressCategory.CategoryId));
                    }

                    public static MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>> ByFirmAddress(FindSpecification<Facts::FirmAddress> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>(
                            q => from firm in q.For<Facts::Firm>()
                                 join project in q.For<Facts::Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                 join firmAddress in q.For(specification) on firm.Id equals firmAddress.FirmId
                                 join firmAddressCategory in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                 select new Tuple<long, long?>(project.Id, firmAddressCategory.CategoryId));
                    }

                    public static MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>> ByFirmAddressCategory(FindSpecification<Facts::CategoryFirmAddress> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>(
                            q => from firm in q.For<Facts::Firm>()
                                 join project in q.For<Facts::Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                 join firmAddress in q.For<Facts::FirmAddress>() on firm.Id equals firmAddress.FirmId
                                 join firmAddressCategory in q.For(specification) on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                 select new Tuple<long, long?>(project.Id, firmAddressCategory.CategoryId));
                    }

                    public static MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>> ByProject(FindSpecification<Facts::Project> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>(
                            q => from project in q.For(specification)
                                 select new Tuple<long, long?>(project.Id, null));
                    }
                }
            }
        }
    }
}