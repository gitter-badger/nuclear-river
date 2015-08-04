using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Erm
        {
            public static class Map
            {
                public static class ToFacts
                {
                    public static MapSpecification<IQuery, IQueryable<Account>> Accounts()
                    {
                        return new MapSpecification<IQuery, IQueryable<Account>>(
                            q => from account in q.For(Find.Accounts())
                                 select new Account
                                 {
                                     Id = account.Id,
                                     Balance = account.Balance,
                                     BranchOfficeOrganizationUnitId = account.BranchOfficeOrganizationUnitId,
                                     LegalPersonId = account.LegalPersonId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<BranchOfficeOrganizationUnit>> BranchOfficeOrganizationUnits()
                    {
                        return new MapSpecification<IQuery, IQueryable<BranchOfficeOrganizationUnit>>(
                            q => from branchOfficeOrganizationUnit in q.For(Find.BranchOfficeOrganizationUnits())
                                 select new BranchOfficeOrganizationUnit
                                        {
                                            Id = branchOfficeOrganizationUnit.Id,
                                            OrganizationUnitId = branchOfficeOrganizationUnit.OrganizationUnitId
                                        });
                    }

                    public static MapSpecification<IQuery, IQueryable<Category>> Categories()
                    {
                        return new MapSpecification<IQuery, IQueryable<Category>>(
                            q => from category in q.For(Find.Categories())
                                 select new Category
                                 {
                                     Id = category.Id,
                                     Name = category.Name,
                                     Level = category.Level,
                                     ParentId = category.ParentId
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<CategoryGroup>> CategoryGroups()
                    {
                        return new MapSpecification<IQuery, IQueryable<CategoryGroup>>(
                            q => from categoryGroup in q.For(Find.CategoryGroups())
                                 select new CategoryGroup
                                 {
                                     Id = categoryGroup.Id,
                                     Name = categoryGroup.Name,
                                     Rate = categoryGroup.Rate
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<CategoryFirmAddress>> CategoryFirmAddresses()
                    {
                        return new MapSpecification<IQuery, IQueryable<CategoryFirmAddress>>(
                            q => from categoryFirmAddress in q.For(Find.CategoryFirmAddresses())
                                 select new CategoryFirmAddress
                                 {
                                     Id = categoryFirmAddress.Id,
                                     CategoryId = categoryFirmAddress.CategoryId,
                                     FirmAddressId = categoryFirmAddress.FirmAddressId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<CategoryOrganizationUnit>> CategoryOrganizationUnits()
                    {
                        return new MapSpecification<IQuery, IQueryable<CategoryOrganizationUnit>>(
                            q => from categoryOrganizationUnit in q.For(Find.CategoryOrganizationUnits())
                                 select new CategoryOrganizationUnit
                                 {
                                     Id = categoryOrganizationUnit.Id,
                                     CategoryId = categoryOrganizationUnit.CategoryId,
                                     CategoryGroupId = categoryOrganizationUnit.CategoryGroupId,
                                     OrganizationUnitId = categoryOrganizationUnit.OrganizationUnitId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Client>> Clients()
                    {
                        return new MapSpecification<IQuery, IQueryable<Client>>(
                            q => from client in q.For(Find.Clients())
                                 select new Client
                                 {
                                     Id = client.Id,
                                     Name = client.Name,
                                     LastDisqualifiedOn = client.LastDisqualifyTime,
                                     HasPhone = (client.MainPhoneNumber ?? client.AdditionalPhoneNumber1 ?? client.AdditionalPhoneNumber2) != null,
                                     HasWebsite = client.Website != null
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Contact>> Contacts()
                    {
                        return new MapSpecification<IQuery, IQueryable<Contact>>(
                            q => from contact in q.For(Find.Contacts())
                                 select new Contact
                                 {
                                     Id = contact.Id,
                                     Role = ConvertAccountRole(contact.Role),
                                     IsFired = contact.IsFired,
                                     HasPhone = (contact.MainPhoneNumber ?? contact.MobilePhoneNumber ?? contact.HomePhoneNumber ?? contact.AdditionalPhoneNumber) != null,
                                     HasWebsite = contact.Website != null,
                                     ClientId = contact.ClientId
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Firm>> Firms()
                    {
                        return new MapSpecification<IQuery, IQueryable<Firm>>(
                            q => from firm in q.For(Find.Firms())
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
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<FirmAddress>> FirmAddresses()
                    {
                        return new MapSpecification<IQuery, IQueryable<FirmAddress>>(
                            q => from firmAddress in q.For(Find.FirmAddresses())
                                 select new FirmAddress
                                 {
                                     Id = firmAddress.Id,
                                     FirmId = firmAddress.FirmId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<FirmContact>> FirmContacts()
                    {
                        return new MapSpecification<IQuery, IQueryable<FirmContact>>(
                            q => from firmContact in q.For(Find.FirmContacts())
                                 where firmContact.FirmAddressId != null && (firmContact.ContactType == ContactType.Phone || firmContact.ContactType == ContactType.Website)
                                 select new FirmContact
                                 {
                                     Id = firmContact.Id,
                                     HasPhone = firmContact.ContactType == ContactType.Phone,
                                     HasWebsite = firmContact.ContactType == ContactType.Website,
                                     FirmAddressId = (long)firmContact.FirmAddressId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<LegalPerson>> LegalPersons()
                    {
                        return new MapSpecification<IQuery, IQueryable<LegalPerson>>(
                            q => from legalPerson in q.For(Find.LegalPersons())
                                 where legalPerson.ClientId != null
                                 select new LegalPerson
                                 {
                                     Id = legalPerson.Id,
                                     ClientId = (long)legalPerson.ClientId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Order>> Orders()
                    {
                        return new MapSpecification<IQuery, IQueryable<Order>>(
                            q => from order in q.For(Find.Orders())
                                 where new[] { OrderState.OnTermination, OrderState.Archive }.Contains(order.WorkflowStepId)
                                 select new Order
                                        {
                                            Id = order.Id,
                                            EndDistributionDateFact = order.EndDistributionDateFact,
                                            FirmId = order.FirmId,
                                        });
                    }

                    public static MapSpecification<IQuery, IQueryable<Project>> Projects()
                    {
                        return new MapSpecification<IQuery, IQueryable<Project>>(
                            q => from project in q.For(Find.Projects())
                                 select new Project
                                 {
                                     Id = project.Id,
                                     Name = project.Name,
                                     OrganizationUnitId = project.OrganizationUnitId
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Territory>> Territories()
                    {
                        return new MapSpecification<IQuery, IQueryable<Territory>>(
                            q => from territory in q.For(Find.Territories())
                                 select new Territory
                                 {
                                     Id = territory.Id,
                                     Name = territory.Name,
                                     OrganizationUnitId = territory.OrganizationUnitId
                                 });
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
        }

        public static class Facts
        {
            public static class Map
            {
                public static class ToClientAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirm(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firm in q.For(Find.ByIds<Firm>(ids))
                                    where firm.ClientId != null
                                    select firm.ClientId.Value);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByContacts(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from client in q.For<Client>()
                                    join contact in q.For(Find.ByIds<Contact>(ids)) on client.Id equals contact.ClientId
                                    select client.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryFirmAddress(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryFirmAddress in q.For(Find.ByIds<CategoryFirmAddress>(ids))
                                 join firmAddress in q.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                 join firm in q.For<Firm>() on firmAddress.FirmId equals firm.Id
                                 where firm.ClientId.HasValue
                                 select firm.ClientId.Value);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryGroup(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from categoryGroup in q.For(Find.ByIds<CategoryGroup>(ids))
                                  join categoryOrganizationUnit in q.For<CategoryOrganizationUnit>() on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                                  join categoryFirmAddress in q.For<CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                                  join firmAddress in q.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                  join firm in q.For<Firm>()
                                      on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals new { firm.OrganizationUnitId, FirmId = firm.Id }
                                  where firm.ClientId.HasValue
                                  select firm.ClientId.Value).Distinct());
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryOrganizationUnit(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryOrganizationUnit in q.For(Find.ByIds<CategoryOrganizationUnit>(ids))
                                 join categoryFirmAddress in q.For<CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                                 join firmAddress in q.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                 join firm in q.For<Firm>()
                                     on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals new { firm.OrganizationUnitId, FirmId = firm.Id }
                                 where firm.ClientId.HasValue
                                 select firm.ClientId.Value);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmAddress(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firmAddress in q.For(Find.ByIds<FirmAddress>(ids))
                                 join firm in q.For<Firm>() on firmAddress.FirmId equals firm.Id
                                 where firm.ClientId.HasValue
                                 select firm.ClientId.Value);
                    }
                }

                public static class ToFirmAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByAccount(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from account in q.For(Find.ByIds<Account>(ids))
                                 join legalPerson in q.For<LegalPerson>() on account.LegalPersonId equals legalPerson.Id
                                 join client in q.For<Client>() on legalPerson.ClientId equals client.Id
                                 join branchOfficeOrganizationUnit in q.For<BranchOfficeOrganizationUnit>()
                                     on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                                 join firm in q.For<Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 where firm.ClientId == client.Id
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByBranchOfficeOrganizationUnit(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from branchOfficeOrganizationUnit in q.For(Find.ByIds<BranchOfficeOrganizationUnit>(ids))
                                 join firm in q.For<Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategory(IEnumerable<long> ids)
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

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryFirmAddress(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryFirmAddress in q.For(Find.ByIds<CategoryFirmAddress>(ids))
                                 join firmAddress in q.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                 select firmAddress.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryFirmAddressForStatistics(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var changeKeys = from firm in q.For<Firm>()
                                                 join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                                 join firmAddressCategory in q.For(Find.ByIds<CategoryFirmAddress>(ids)) on firmAddress.Id equals firmAddressCategory.FirmAddressId
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

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryOrganizationUnit(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from firm in q.For<Firm>()
                                  join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                  join categoryFirmAddress in q.For<CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                  join categoryOrganizationUnit in q.For(Find.ByIds<CategoryOrganizationUnit>(ids))
                                  on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId
                                  where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                                  select firmAddress.FirmId).Distinct());
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByClient(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firm in q.For(new FindSpecification<Firm>(x => ids.Contains(x.ClientId.Value)))
                                 where firm.ClientId != null
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByContacts(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firm in q.For<Firm>()
                                 join client in q.For<Client>() on firm.ClientId equals client.Id
                                 join contact in q.For(Find.ByIds<Contact>(ids)) on client.Id equals contact.ClientId
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmForStatistics(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var changeKeys = from firm in q.For(Find.ByIds<Firm>(ids))
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

                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryGroup(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from categoryGroup in q.For(Find.ByIds<CategoryGroup>(ids))
                                  join categoryOrganizationUnit in q.For<CategoryOrganizationUnit>() on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                                  join categoryFirmAddress in q.For<CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                                  join firmAddress in q.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                                  select firmAddress.FirmId).Distinct());
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmAddress(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firmAddress in q.For(Find.ByIds<FirmAddress>(ids))
                                 select firmAddress.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmAddressForStatistics(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                            {
                                var changeKeys = from firm in q.For<Firm>()
                                                 join firmAddress in q.For(Find.ByIds<FirmAddress>(ids)) on firm.Id equals firmAddress.FirmId
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

                    public static MapSpecification<IQuery, IEnumerable<long>> ByFirmContacts(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from firmAddress in q.For<FirmAddress>()
                                 join firmContact in q.For(Find.ByIds<FirmContact>(ids)) on firmAddress.Id equals firmContact.FirmAddressId
                                 select firmAddress.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByLegalPerson(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from account in q.For<Account>()
                                 join legalPerson in q.For(Find.ByIds<LegalPerson>(ids)) on account.LegalPersonId equals legalPerson.Id
                                 join client in q.For<Client>() on legalPerson.ClientId equals client.Id
                                 join branchOfficeOrganizationUnit in q.For<BranchOfficeOrganizationUnit>()
                                     on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                                 join firm in q.For<Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                                 where firm.ClientId == client.Id
                                 select firm.Id);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByOrder(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from order in q.For(Find.ByIds<Order>(ids))
                                 select order.FirmId);
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByProject(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from project in q.For(Find.ByIds<Project>(ids))
                                 join firm in q.For<Firm>() on project.OrganizationUnitId equals firm.OrganizationUnitId
                                 select firm.Id);
                    }
                }

                public static class ToProjectAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategoryOrganizationUnit(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from categoryOrganizationUnit in q.For(Find.ByIds<CategoryOrganizationUnit>(ids))
                                 join project in q.For<Project>() on categoryOrganizationUnit.OrganizationUnitId equals project.OrganizationUnitId
                                 select project.Id);
                    }
                }

                public static class ToTerritoryAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByProject(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => from project in q.For(Find.ByIds<Project>(ids))
                                 join territory in q.For<Territory>() on project.OrganizationUnitId equals territory.OrganizationUnitId
                                 select territory.Id);
                    }
                }

                public static class ToStatistics
                {
                    public static MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>> ByFirm(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>>(
                            q => from firm in q.For(Find.ByIds<Firm>(ids))
                                 join project in q.For<Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                 join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                 join firmAddressCategory in q.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                 select new CalculateStatisticsOperation { CategoryId = firmAddressCategory.CategoryId, ProjectId = project.Id });
                    }

                    public static MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>> ByFirmAddress(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>>(
                            q => from firm in q.For<Firm>()
                                 join project in q.For<Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                 join firmAddress in q.For(Find.ByIds<FirmAddress>(ids)) on firm.Id equals firmAddress.FirmId
                                 join firmAddressCategory in q.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                 select new CalculateStatisticsOperation { CategoryId = firmAddressCategory.CategoryId, ProjectId = project.Id });
                    }

                    public static MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>> ByFirmAddressCategory(IEnumerable<long> ids)
                    {
                        return new MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>>(
                            q => from firm in q.For<Firm>()
                                 join project in q.For<Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                 join firmAddress in q.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                 join firmAddressCategory in q.For(Find.ByIds<CategoryFirmAddress>(ids)) on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                 select new CalculateStatisticsOperation { CategoryId = firmAddressCategory.CategoryId, ProjectId = project.Id });
                    }

                    public static MapSpecification<IQuery, IEnumerable<CalculateStatisticsOperation>> ByProject(IEnumerable<long> ids)
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