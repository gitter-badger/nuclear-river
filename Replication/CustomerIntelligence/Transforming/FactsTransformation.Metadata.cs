using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

    public sealed partial class FactsTransformation
    {
        private static readonly Dictionary<Type, FactInfo> Facts
            = new FactInfo[]
              {
                  FactInfo.OfType<Account>()
                          .HasSource(context => context.Accounts, Filter.ById)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByAccount),

                  FactInfo.OfType<BranchOfficeOrganizationUnit>()
                          .HasSource(context => context.BranchOfficeOrganizationUnits, Filter.ById)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByBranchOfficeOrganizationUnit),

                  FactInfo.OfType<Category>()
                          .HasSource(context => context.Categories, Filter.ById)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategory),

                  FactInfo.OfType<CategoryFirmAddress>()
                          .HasSource(context => context.CategoryFirmAddresses, Filter.ById)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategoryFirmAddress),

                  FactInfo.OfType<CategoryGroup>()
                          .HasSource(context => context.CategoryGroups, Filter.ById)
                          .HasMatchedAggregate<CI.CategoryGroup>(),

                  FactInfo.OfType<CategoryOrganizationUnit>()
                          .HasSource(context => context.CategoryOrganizationUnits, Filter.ById)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategoryOrganizationUnit),

                  FactInfo.OfType<Client>()
                          .HasSource(context => context.Clients, Filter.ById)
                          .HasMatchedAggregate<CI.Client>()
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByClient),

                  FactInfo.OfType<Contact>()
                          .HasSource(context => context.Contacts, Filter.ById)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByContacts),

                  FactInfo.OfType<Firm>()
                          .HasSource(context => context.Firms, Filter.ById)
                          .HasMatchedAggregate<CI.Firm>()
                          .HasDependentAggregate<CI.Client>(Find.Client.ByFirm),

                  FactInfo.OfType<FirmAddress>()
                          .HasSource(context => context.FirmAddresses, Filter.ById)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByFirmAddress),

                  FactInfo.OfType<FirmContact>()
                          .HasSource(context => context.FirmContacts, Filter.ById)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByFirmContacts),

                  FactInfo.OfType<LegalPerson>()
                          .HasSource(context => context.LegalPersons, Filter.ById)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByLegalPerson),

                  FactInfo.OfType<Order>()
                          .HasSource(context => context.Orders, Filter.ById)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByOrder),

                  FactInfo.OfType<Project>()
                          .HasSource(context => context.Projects, Filter.ById)
                          .HasMatchedAggregate<CI.Project>()
                          .HasDependentAggregate<CI.Territory>(Find.Territory.ByProject)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByProject),

                  FactInfo.OfType<Territory>()
                          .HasSource(context => context.Territories, Filter.ById)
                          .HasMatchedAggregate<CI.Territory>(),

              }.ToDictionary(x => x.FactType);

        private static class Filter
        {
            public static IQueryable<TFact> ById<TFact>(IQueryable<TFact> queryable, IEnumerable<long> ids) where TFact : IIdentifiableObject
            {
                return queryable.Where(fact => ids.Contains(fact.Id));
            }
        }

        private static class Find
        {
            public static class Client
            {
                public static IEnumerable<long> ByFirm(IFactsContext context, IEnumerable<long> ids)
                {
                    return from firm in context.Firms
                           where ids.Contains(firm.Id) && firm.ClientId != null
                           select firm.ClientId.Value;
                }
            }

            public static class Firm
            {
                public static IEnumerable<long> ByAccount(IFactsContext context, IEnumerable<long> ids)
                {
                    return from account in context.Accounts.Where(x => ids.Contains(x.Id))
                           join legalPerson in context.LegalPersons on account.LegalPersonId equals legalPerson.Id
                           join client in context.Clients on legalPerson.ClientId equals client.Id
                           join branchOfficeOrganizationUnit in context.BranchOfficeOrganizationUnits on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                           join firm in context.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                           where firm.ClientId == client.Id
                           select firm.Id;
                }

                public static IEnumerable<long> ByBranchOfficeOrganizationUnit(IFactsContext context, IEnumerable<long> ids)
                {
                    return from branchOfficeOrganizationUnit in context.BranchOfficeOrganizationUnits.Where(x => ids.Contains(x.Id))
                           join firm in context.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                           select firm.Id;
                }

                public static IEnumerable<long> ByCategory(IFactsContext context, IEnumerable<long> ids)
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

                public static IEnumerable<long> ByCategoryFirmAddress(IFactsContext context, IEnumerable<long> ids)
                {
                    return from categoryFirmAddress in context.CategoryFirmAddresses
                           join firmAddress in context.FirmAddresses on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                           where ids.Contains(categoryFirmAddress.Id)
                           select firmAddress.FirmId;
                }

                public static IEnumerable<long> ByCategoryOrganizationUnit(IFactsContext context, IEnumerable<long> ids)
                {
                    return (from firm in context.Firms
                            join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                            join categoryFirmAddress in context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                            join categoryOrganizationUnit in context.CategoryOrganizationUnits.Where(x => ids.Contains(x.Id)) on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId
                            where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                            select firmAddress.FirmId).Distinct();
                }

                public static IEnumerable<long> ByClient(IFactsContext context, IEnumerable<long> ids)
                {
                    return from firm in context.Firms
                           where firm.ClientId != null && ids.Contains(firm.ClientId.Value)
                           select firm.Id;
                }

                public static IEnumerable<long> ByContacts(IFactsContext context, IEnumerable<long> ids)
                {
                    return from firm in context.Firms
                           join client in context.Clients on firm.ClientId equals client.Id
                           join contact in context.Contacts on client.Id equals contact.ClientId
                           where ids.Contains(contact.Id)
                           select firm.Id;
                }

                public static IEnumerable<long> ByFirmAddress(IFactsContext context, IEnumerable<long> ids)
                {
                    return from firmAddress in context.FirmAddresses
                           where ids.Contains(firmAddress.Id)
                           select firmAddress.FirmId;
                }

                public static IEnumerable<long> ByFirmContacts(IFactsContext context, IEnumerable<long> ids)
                {
                    return from firmAddress in context.FirmAddresses
                           join firmContact in context.FirmContacts on firmAddress.Id equals firmContact.FirmAddressId
                           where ids.Contains(firmContact.Id)
                           select firmAddress.FirmId;
                }

                public static IEnumerable<long> ByLegalPerson(IFactsContext context, IEnumerable<long> ids)
                {
                    return from account in context.Accounts
                           join legalPerson in context.LegalPersons.Where(x => ids.Contains(x.Id)) on account.LegalPersonId equals legalPerson.Id
                           join client in context.Clients on legalPerson.ClientId equals client.Id
                           join branchOfficeOrganizationUnit in context.BranchOfficeOrganizationUnits on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                           join firm in context.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                           where firm.ClientId == client.Id
                           select firm.Id;
                }

                public static IEnumerable<long> ByOrder(IFactsContext context, IEnumerable<long> ids)
                {
                    return from order in context.Orders
                           where ids.Contains(order.Id)
                           select order.FirmId;
                }

                public static IEnumerable<long> ByProject(IFactsContext context, IEnumerable<long> ids)
                {
                    return from project in context.Projects
                           join firm in context.Firms on project.OrganizationUnitId equals firm.OrganizationUnitId
                           where ids.Contains(project.Id)
                           select firm.Id;
                }
            }

            public static class Territory
            {
                public static IEnumerable<long> ByProject(IFactsContext context, IEnumerable<long> ids)
                {
                    var x =  from project in context.Projects
                           join territory in context.Territories on project.OrganizationUnitId equals territory.OrganizationUnitId
                           where ids.Contains(project.Id)
                           select territory.Id;
                    var y = x.ToArray();
                    return y;
                }
            }
        }
    }
}