using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

    public sealed partial class ErmFactsTransformation
    {
        // Правило по определению зависимых агрегатов: смотрим сборку CI сущностей из фактов (CustomerIntelligenceTransformationContext)
        // если видим join - считаем, что агрегат зависит от факта, если join'а нет - то нет (даже при наличии связи по Id)
        private static readonly Dictionary<Type, ErmFactInfo> Facts
            = new ErmFactInfo[]
              {
                  ErmFactInfo.OfType<Activity>()
                          .HasSource(context => context.Activities)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByActivity)
                          .HasDependentAggregate<CI.Client>(Find.Client.ByActivity),

                  ErmFactInfo.OfType<Account>()
                          .HasSource(context => context.Accounts)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByAccount),

                  ErmFactInfo.OfType<BranchOfficeOrganizationUnit>()
                          .HasSource(context => context.BranchOfficeOrganizationUnits)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByBranchOfficeOrganizationUnit),

                  ErmFactInfo.OfType<Category>()
                          .HasSource(context => context.Categories)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategory),

                  ErmFactInfo.OfType<CategoryFirmAddress>()
                          .HasSource(context => context.CategoryFirmAddresses)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategoryFirmAddress)
                          //.HasDependentAggregate<CI.Firm>(Find.Firm.ByCategoryFirmAddressForStatistics)
                          .HasDependentAggregate<CI.Client>(Find.Client.ByCategoryFirmAddress),

                  ErmFactInfo.OfType<CategoryGroup>()
                          .HasSource(context => context.CategoryGroups)
                          .HasMatchedAggregate<CI.CategoryGroup>()
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategoryGroup)
                          .HasDependentAggregate<CI.Client>(Find.Client.ByCategoryGroup),

                  ErmFactInfo.OfType<CategoryOrganizationUnit>()
                          .HasSource(context => context.CategoryOrganizationUnits)
                          .HasDependentAggregate<CI.Project>(Find.Project.ByCategoryOrganizationUnit)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategoryOrganizationUnit)
                          .HasDependentAggregate<CI.Client>(Find.Client.ByCategoryOrganizationUnit),

                  ErmFactInfo.OfType<Client>()
                          .HasSource(context => context.Clients)
                          .HasMatchedAggregate<CI.Client>()
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByClient),

                  ErmFactInfo.OfType<Contact>()
                          .HasSource(context => context.Contacts)
                          .HasDependentAggregate<CI.Client>(Find.Client.ByContacts)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByContacts),

                  ErmFactInfo.OfType<Firm>()
                          .HasSource(context => context.Firms)
                          .HasMatchedAggregate<CI.Firm>()
                          //.HasDependentAggregate<CI.Firm>(Find.Firm.ByFirmForStatistics)
                          .HasDependentAggregate<CI.Client>(Find.Client.ByFirm),

                  ErmFactInfo.OfType<FirmAddress>()
                          .HasSource(context => context.FirmAddresses)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByFirmAddress)
                          //.HasDependentAggregate<CI.Firm>(Find.Firm.ByFirmAddressForStatistics)
                          .HasDependentAggregate<CI.Client>(Find.Client.ByFirmAddress),

                  ErmFactInfo.OfType<FirmContact>()
                          .HasSource(context => context.FirmContacts)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByFirmContacts),

                  ErmFactInfo.OfType<LegalPerson>()
                          .HasSource(context => context.LegalPersons)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByLegalPerson),

                  ErmFactInfo.OfType<Order>()
                          .HasSource(context => context.Orders)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByOrder),

                  ErmFactInfo.OfType<Project>()
                          .HasSource(context => context.Projects)
                          .HasMatchedAggregate<CI.Project>()
                          .HasDependentAggregate<CI.Territory>(Find.Territory.ByProject)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByProject),

                  ErmFactInfo.OfType<Territory>()
                          .HasSource(context => context.Territories)
                          .HasMatchedAggregate<CI.Territory>(),

              }.ToDictionary(x => x.FactType);

        private static class Find
        {
            public static class Client
            {
                public static IEnumerable<long> ByFirm(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from firm in context.Firms
                           where ids.Contains(firm.Id) && firm.ClientId != null
                           select firm.ClientId.Value;
                }

                public static IEnumerable<long> ByContacts(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from client in context.Clients
                           join contact in context.Contacts on client.Id equals contact.ClientId
                           where ids.Contains(contact.Id)
                           select client.Id;
                }

                public static IEnumerable<long> ByCategoryFirmAddress(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from categoryFirmAddress in context.CategoryFirmAddresses
                           join firmAddress in context.FirmAddresses on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                           join firm in context.Firms on firmAddress.FirmId equals firm.Id
                           where ids.Contains(categoryFirmAddress.Id) && firm.ClientId.HasValue
                           select firm.ClientId.Value;
                }

                public static IEnumerable<long> ByFirmAddress(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from firmAddress in context.FirmAddresses
                           join firm in context.Firms on firmAddress.FirmId equals firm.Id
                           where ids.Contains(firmAddress.Id) && firm.ClientId.HasValue
                           select firm.ClientId.Value;
                }

                public static IEnumerable<long> ByCategoryOrganizationUnit(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from categoryOrganizationUnit in context.CategoryOrganizationUnits
                           join categoryFirmAddress in context.CategoryFirmAddresses on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                           join firmAddress in context.FirmAddresses on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                           join firm in context.Firms on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals
                               new { firm.OrganizationUnitId, FirmId = firm.Id }
                           where ids.Contains(categoryOrganizationUnit.Id) && firm.ClientId.HasValue
                           select firm.ClientId.Value;
                }

                public static IEnumerable<long> ByCategoryGroup(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return (from categoryGroup in context.CategoryGroups
                            join categoryOrganizationUnit in context.CategoryOrganizationUnits on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                            join categoryFirmAddress in context.CategoryFirmAddresses on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                            join firmAddress in context.FirmAddresses on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                            join firm in context.Firms on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals
                                new { firm.OrganizationUnitId, FirmId = firm.Id }
                            where ids.Contains(categoryGroup.Id) && firm.ClientId.HasValue
                            select firm.ClientId.Value).Distinct();
                }

                public static IEnumerable<long> ByActivity(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from activity in context.Activities
                           where ids.Contains(activity.Id) && activity.ClientId.HasValue
                           select activity.ClientId.Value;
                }
            }

            public static class Firm
            {
                public static IEnumerable<long> ByAccount(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from account in context.Accounts.Where(x => ids.Contains(x.Id))
                           join legalPerson in context.LegalPersons on account.LegalPersonId equals legalPerson.Id
                           join client in context.Clients on legalPerson.ClientId equals client.Id
                           join branchOfficeOrganizationUnit in context.BranchOfficeOrganizationUnits on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                           join firm in context.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                           where firm.ClientId == client.Id
                           select firm.Id;
                }

                public static IEnumerable<long> ByBranchOfficeOrganizationUnit(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from branchOfficeOrganizationUnit in context.BranchOfficeOrganizationUnits.Where(x => ids.Contains(x.Id))
                           join firm in context.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                           select firm.Id;
                }

                public static IEnumerable<long> ByCategory(IErmFactsContext context, IEnumerable<long> ids)
                {
                    var categories1 = context.Categories.Where(x => x.Level == 1);
                    var categories2 = context.Categories.Where(x => x.Level == 2);
                    var categories3 = context.Categories.Where(x => x.Level == 3);

                    var level3 = from firmAddress in context.FirmAddresses
                                 join categoryFirmAddress in context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                 join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                 where ids.Contains(category3.Id)
                                 select firmAddress.FirmId;

                    var level2 = from firmAddress in context.FirmAddresses
                                 join categoryFirmAddress in context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                 join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                 join category2 in categories2 on category3.ParentId equals category2.Id
                                 where ids.Contains(category2.Id)
                                 select firmAddress.FirmId;

                    var level1 = from firmAddress in context.FirmAddresses
                                 join categoryFirmAddress in context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                 join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                 join category2 in categories2 on category3.ParentId equals category2.Id
                                 join category1 in categories1 on category2.ParentId equals category1.Id
                                 where ids.Contains(category1.Id)
                                 select firmAddress.FirmId;

                    return level3.Union(level2).Union(level1);
                }

                public static IEnumerable<long> ByCategoryFirmAddress(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from categoryFirmAddress in context.CategoryFirmAddresses
                           join firmAddress in context.FirmAddresses on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                           where ids.Contains(categoryFirmAddress.Id)
                           select firmAddress.FirmId;
                }

                public static IEnumerable<long> ByCategoryFirmAddressForStatistics(IErmFactsContext context, IEnumerable<long> ids)
                {
                    var changeKeys = from firm in context.Firms
                                     join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                                     join firmAddressCategory in context.CategoryFirmAddresses on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                     where ids.Contains(firmAddressCategory.Id)
                                     select new { firm.OrganizationUnitId, firmAddressCategory.CategoryId };

                    var query = from firm in context.Firms
                                join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                                join firmAddressCategory in context.CategoryFirmAddresses on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                join key in changeKeys on new { firm.OrganizationUnitId, firmAddressCategory.CategoryId } equals new { key.OrganizationUnitId, key.CategoryId }
                                select firm.Id;

                    return query.Distinct();
                }

                public static IEnumerable<long> ByCategoryOrganizationUnit(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return (from firm in context.Firms
                            join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                            join categoryFirmAddress in context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                            join categoryOrganizationUnit in context.CategoryOrganizationUnits.Where(x => ids.Contains(x.Id)) on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId
                            where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                            select firmAddress.FirmId).Distinct();
                }

                public static IEnumerable<long> ByClient(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from firm in context.Firms
                           where firm.ClientId != null && ids.Contains(firm.ClientId.Value)
                           select firm.Id;
                }

                public static IEnumerable<long> ByContacts(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from firm in context.Firms
                           join client in context.Clients on firm.ClientId equals client.Id
                           join contact in context.Contacts on client.Id equals contact.ClientId
                           where ids.Contains(contact.Id)
                           select firm.Id;
                }

                public static IEnumerable<long> ByFirmForStatistics(IErmFactsContext context, IEnumerable<long> ids)
                {
                    var changeKeys = from firm in context.Firms
                                     join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                                     join firmAddressCategory in context.CategoryFirmAddresses on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                     where ids.Contains(firm.Id)
                                     select new { firm.OrganizationUnitId, firmAddressCategory.CategoryId };

                    var query = from firm in context.Firms
                                join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                                join firmAddressCategory in context.CategoryFirmAddresses on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                join key in changeKeys on new { firm.OrganizationUnitId, firmAddressCategory.CategoryId } equals new { key.OrganizationUnitId, key.CategoryId }
                                select firm.Id;

                    return query.Distinct();
                }

                public static IEnumerable<long> ByFirmAddress(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from firmAddress in context.FirmAddresses
                           where ids.Contains(firmAddress.Id)
                           select firmAddress.FirmId;
                }

                public static IEnumerable<long> ByFirmAddressForStatistics(IErmFactsContext context, IEnumerable<long> ids)
                {
                    var changeKeys = from firm in context.Firms
                                     join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                                     join firmAddressCategory in context.CategoryFirmAddresses on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                     where ids.Contains(firmAddress.Id)
                                     select new { firm.OrganizationUnitId, firmAddressCategory.CategoryId };

                    var query = from firm in context.Firms
                                join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                                join firmAddressCategory in context.CategoryFirmAddresses on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                join key in changeKeys on new { firm.OrganizationUnitId, firmAddressCategory.CategoryId } equals new { key.OrganizationUnitId, key.CategoryId }
                                select firm.Id;

                    return query.Distinct();
                }

                public static IEnumerable<long> ByFirmContacts(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from firmAddress in context.FirmAddresses
                           join firmContact in context.FirmContacts on firmAddress.Id equals firmContact.FirmAddressId
                           where ids.Contains(firmContact.Id)
                           select firmAddress.FirmId;
                }

                public static IEnumerable<long> ByLegalPerson(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from account in context.Accounts
                           join legalPerson in context.LegalPersons.Where(x => ids.Contains(x.Id)) on account.LegalPersonId equals legalPerson.Id
                           join client in context.Clients on legalPerson.ClientId equals client.Id
                           join branchOfficeOrganizationUnit in context.BranchOfficeOrganizationUnits on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                           join firm in context.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                           where firm.ClientId == client.Id
                           select firm.Id;
                }

                public static IEnumerable<long> ByOrder(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from order in context.Orders
                           where ids.Contains(order.Id)
                           select order.FirmId;
                }

                public static IEnumerable<long> ByProject(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from project in context.Projects
                           join firm in context.Firms on project.OrganizationUnitId equals firm.OrganizationUnitId
                           where ids.Contains(project.Id)
                           select firm.Id;
                }

                public static IEnumerable<long> ByCategoryGroup(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return (from categoryGroup in context.CategoryGroups
                            join categoryOrganizationUnit in context.CategoryOrganizationUnits on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                            join categoryFirmAddress in context.CategoryFirmAddresses on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                            join firmAddress in context.FirmAddresses on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                            where ids.Contains(categoryGroup.Id)
                            select firmAddress.FirmId).Distinct();
                }

                public static IEnumerable<long> ByActivity(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from activity in context.Activities
                           where ids.Contains(activity.Id) && activity.FirmId.HasValue
                           select activity.FirmId.Value;
                }
            }

            public static class Territory
            {
                public static IEnumerable<long> ByProject(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from project in context.Projects
                           join territory in context.Territories on project.OrganizationUnitId equals territory.OrganizationUnitId
                           where ids.Contains(project.Id)
                           select territory.Id;
                }
            }

            public static class Project
            {
                public static IEnumerable<long> ByCategoryOrganizationUnit(IErmFactsContext context, IEnumerable<long> ids)
                {
                    return from categoryOrganizationUnit in context.CategoryOrganizationUnits
                           join project in context.Projects on categoryOrganizationUnit.OrganizationUnitId equals project.OrganizationUnitId
                           where ids.Contains(categoryOrganizationUnit.Id)
                           select project.Id;
                }
            }
        }
    }
}