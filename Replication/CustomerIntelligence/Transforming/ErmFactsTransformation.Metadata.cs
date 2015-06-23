using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = Model;
    using Facts = Model.Facts;

    public sealed partial class ErmFactsTransformation
    {
        // Правило по определению зависимых агрегатов: смотрим сборку CI сущностей из фактов (CustomerIntelligenceTransformationContext)
        // если видим join - считаем, что агрегат зависит от факта, если join'а нет - то нет (даже при наличии связи по Id)
        private static readonly Dictionary<Type, ErmFactInfo> Facts
            = new ErmFactInfo[]
              {
                  ErmFactInfo.OfType<Facts.Account>()
                          .HasSource(Specs.Erm.Map.ToFacts.Accounts())
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByAccount),

                  ErmFactInfo.OfType<Facts.BranchOfficeOrganizationUnit>()
                          .HasSource(Specs.Erm.Map.ToFacts.BranchOfficeOrganizationUnits())
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByBranchOfficeOrganizationUnit),

                  ErmFactInfo.OfType<Facts.Category>()
                          .HasSource(Specs.Erm.Map.ToFacts.Categories())
                          .HasMatchedAggregate<CI.Category>()
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategory),

                  ErmFactInfo.OfType<Facts.CategoryFirmAddress>()
                          .HasSource(Specs.Erm.Map.ToFacts.CategoryFirmAddresses())
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategoryFirmAddress),

                  ErmFactInfo.OfType<Facts.CategoryGroup>()
                          .HasSource(Specs.Erm.Map.ToFacts.CategoryGroups())
                          .HasMatchedAggregate<CI.CategoryGroup>()
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategoryGroup)
                          .HasDependentAggregate<CI.Client>(Find.Client.ByCategoryGroup),

                  ErmFactInfo.OfType<Facts.CategoryOrganizationUnit>()
                          .HasSource(Specs.Erm.Map.ToFacts.CategoryOrganizationUnits())
                          .HasDependentAggregate<CI.Project>(Find.Project.ByCategoryOrganizationUnit)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByCategoryOrganizationUnit)
                          .HasDependentAggregate<CI.Client>(Find.Client.ByCategoryOrganizationUnit),

                  ErmFactInfo.OfType<Facts.Client>()
                          .HasSource(Specs.Erm.Map.ToFacts.Clients())
                          .HasMatchedAggregate<CI.Client>()
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByClient),

                  ErmFactInfo.OfType<Facts.Contact>()
                          .HasSource(Specs.Erm.Map.ToFacts.Contacts())
                          .HasDependentAggregate<CI.Client>(Find.Client.ByContacts)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByContacts),

                  ErmFactInfo.OfType<Facts.Firm>()
                          .HasSource(Specs.Erm.Map.ToFacts.Firms())
                          .HasMatchedAggregate<CI.Firm>()
                          .HasDependentAggregate<CI.Client>(Find.Client.ByFirm),

                  ErmFactInfo.OfType<Facts.FirmAddress>()
                          .HasSource(Specs.Erm.Map.ToFacts.FirmAddresses())
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByFirmAddress)
                          .HasDependentAggregate<CI.Client>(Find.Client.ByFirmAddress),

                  ErmFactInfo.OfType<Facts.FirmContact>()
                          .HasSource(Specs.Erm.Map.ToFacts.FirmContacts())
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByFirmContacts),

                  ErmFactInfo.OfType<Facts.LegalPerson>()
                          .HasSource(Specs.Erm.Map.ToFacts.LegalPersons())
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByLegalPerson),

                  ErmFactInfo.OfType<Facts.Order>()
                          .HasSource(Specs.Erm.Map.ToFacts.Orders())
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByOrder),

                  ErmFactInfo.OfType<Facts.Project>()
                          .HasSource(Specs.Erm.Map.ToFacts.Projects())
                          .HasMatchedAggregate<CI.Project>()
                          .HasDependentAggregate<CI.Territory>(Find.Territory.ByProject)
                          .HasDependentAggregate<CI.Firm>(Find.Firm.ByProject),

                  ErmFactInfo.OfType<Facts.Territory>()
                          .HasSource(Specs.Erm.Map.ToFacts.Territories())
                          .HasMatchedAggregate<CI.Territory>(),

              }.ToDictionary(x => x.FactType);

        private static class Find
        {
            public static class Client
            {
                public static IEnumerable<long> ByFirm(IQuery query, IEnumerable<long> ids)
                {
                    return from firm in query.For<Facts.Firm>()
                           where ids.Contains(firm.Id) && firm.ClientId != null
                           select firm.ClientId.Value;
                }

                public static IEnumerable<long> ByContacts(IQuery query, IEnumerable<long> ids)
                {
                    return from client in query.For<Facts.Client>()
                           join contact in query.For<Facts.Contact>() on client.Id equals contact.ClientId
                           where ids.Contains(contact.Id)
                           select client.Id;
                }

                public static IEnumerable<long> ByCategoryFirmAddress(IQuery query, IEnumerable<long> ids)
                {
                    return from categoryFirmAddress in query.For<Facts.CategoryFirmAddress>()
                           join firmAddress in query.For<Facts.FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                           join firm in query.For<Facts.Firm>() on firmAddress.FirmId equals firm.Id
                           where ids.Contains(categoryFirmAddress.Id) && firm.ClientId.HasValue
                           select firm.ClientId.Value;
                }

                public static IEnumerable<long> ByFirmAddress(IQuery query, IEnumerable<long> ids)
                {
                    return from firmAddress in query.For<Facts.FirmAddress>()
                           join firm in query.For<Facts.Firm>() on firmAddress.FirmId equals firm.Id
                           where ids.Contains(firmAddress.Id) && firm.ClientId.HasValue
                           select firm.ClientId.Value;
                }

                public static IEnumerable<long> ByCategoryOrganizationUnit(IQuery query, IEnumerable<long> ids)
                {
                    return from categoryOrganizationUnit in query.For<Facts.CategoryOrganizationUnit>()
                           join categoryFirmAddress in query.For<Facts.CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                           join firmAddress in query.For<Facts.FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                           join firm in query.For<Facts.Firm>() on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals
                               new { firm.OrganizationUnitId, FirmId = firm.Id }
                           where ids.Contains(categoryOrganizationUnit.Id) && firm.ClientId.HasValue
                           select firm.ClientId.Value;
            }

                public static IEnumerable<long> ByCategoryGroup(IQuery query, IEnumerable<long> ids)
                {
                    return (from categoryGroup in query.For<Facts.CategoryGroup>()
                            join categoryOrganizationUnit in query.For<Facts.CategoryOrganizationUnit>() on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                            join categoryFirmAddress in query.For<Facts.CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                            join firmAddress in query.For<Facts.FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                            join firm in query.For<Facts.Firm>() on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals
                                new { firm.OrganizationUnitId, FirmId = firm.Id }
                            where ids.Contains(categoryGroup.Id) && firm.ClientId.HasValue
                            select firm.ClientId.Value).Distinct();
                }
            }

            public static class Firm
            {
                public static IEnumerable<long> ByAccount(IQuery query, IEnumerable<long> ids)
                {
                    return from account in query.For<Facts.Account>().Where(x => ids.Contains(x.Id))
                           join legalPerson in query.For<Facts.LegalPerson>() on account.LegalPersonId equals legalPerson.Id
                           join client in query.For<Facts.Client>() on legalPerson.ClientId equals client.Id
                           join branchOfficeOrganizationUnit in query.For<Facts.BranchOfficeOrganizationUnit>() on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                           join firm in query.For<Facts.Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                           where firm.ClientId == client.Id
                           select firm.Id;
                }

                public static IEnumerable<long> ByBranchOfficeOrganizationUnit(IQuery query, IEnumerable<long> ids)
                {
                    return from branchOfficeOrganizationUnit in query.For<Facts.BranchOfficeOrganizationUnit>().Where(x => ids.Contains(x.Id))
                           join firm in query.For<Facts.Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                           select firm.Id;
                }

                public static IEnumerable<long> ByCategory(IQuery query, IEnumerable<long> ids)
                {
                    var categories1 = query.For<Facts.Category>().Where(x => x.Level == 1);
                    var categories2 = query.For<Facts.Category>().Where(x => x.Level == 2);
                    var categories3 = query.For<Facts.Category>().Where(x => x.Level == 3);

                    var level3 = from firmAddress in query.For<Facts.FirmAddress>()
                                 join categoryFirmAddress in query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                 join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                 where ids.Contains(category3.Id)
                                 select firmAddress.FirmId;

                    var level2 = from firmAddress in query.For<Facts.FirmAddress>()
                                 join categoryFirmAddress in query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                 join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                 join category2 in categories2 on category3.ParentId equals category2.Id
                                 where ids.Contains(category2.Id)
                                 select firmAddress.FirmId;

                    var level1 = from firmAddress in query.For<Facts.FirmAddress>()
                                 join categoryFirmAddress in query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                 join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                                 join category2 in categories2 on category3.ParentId equals category2.Id
                                 join category1 in categories1 on category2.ParentId equals category1.Id
                                 where ids.Contains(category1.Id)
                                 select firmAddress.FirmId;

                    return level3.Union(level2).Union(level1);
                }

                public static IEnumerable<long> ByCategoryFirmAddress(IQuery query, IEnumerable<long> ids)
                {
                    return from categoryFirmAddress in query.For<Facts.CategoryFirmAddress>()
                           join firmAddress in query.For<Facts.FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                           where ids.Contains(categoryFirmAddress.Id)
                           select firmAddress.FirmId;
                }

                public static IEnumerable<long> ByCategoryOrganizationUnit(IQuery query, IEnumerable<long> ids)
                {
                    return (from firm in query.For<Facts.Firm>()
                            join firmAddress in query.For<Facts.FirmAddress>() on firm.Id equals firmAddress.FirmId
                            join categoryFirmAddress in query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                            join categoryOrganizationUnit in query.For<Facts.CategoryOrganizationUnit>().Where(x => ids.Contains(x.Id)) on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId
                            where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                            select firmAddress.FirmId).Distinct();
                }

                public static IEnumerable<long> ByClient(IQuery query, IEnumerable<long> ids)
                {
                    return from firm in query.For<Facts.Firm>()
                           where firm.ClientId != null && ids.Contains(firm.ClientId.Value)
                           select firm.Id;
                }

                public static IEnumerable<long> ByContacts(IQuery query, IEnumerable<long> ids)
                {
                    return from firm in query.For<Facts.Firm>()
                           join client in query.For<Facts.Client>() on firm.ClientId equals client.Id
                           join contact in query.For<Facts.Contact>() on client.Id equals contact.ClientId
                           where ids.Contains(contact.Id)
                           select firm.Id;
                }

                public static IEnumerable<long> ByFirmAddress(IQuery query, IEnumerable<long> ids)
                {
                    return from firmAddress in query.For<Facts.FirmAddress>()
                           where ids.Contains(firmAddress.Id)
                           select firmAddress.FirmId;
                }

                public static IEnumerable<long> ByFirmContacts(IQuery query, IEnumerable<long> ids)
                {
                    return from firmAddress in query.For<Facts.FirmAddress>()
                           join firmContact in query.For<Facts.FirmContact>() on firmAddress.Id equals firmContact.FirmAddressId
                           where ids.Contains(firmContact.Id)
                           select firmAddress.FirmId;
                }

                public static IEnumerable<long> ByLegalPerson(IQuery query, IEnumerable<long> ids)
                {
                    return from account in query.For<Facts.Account>()
                           join legalPerson in query.For<Facts.LegalPerson>().Where(x => ids.Contains(x.Id)) on account.LegalPersonId equals legalPerson.Id
                           join client in query.For<Facts.Client>() on legalPerson.ClientId equals client.Id
                           join branchOfficeOrganizationUnit in query.For<Facts.BranchOfficeOrganizationUnit>() on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                           join firm in query.For<Facts.Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                           where firm.ClientId == client.Id
                           select firm.Id;
                }

                public static IEnumerable<long> ByOrder(IQuery query, IEnumerable<long> ids)
                {
                    return from order in query.For<Facts.Order>()
                           where ids.Contains(order.Id)
                           select order.FirmId;
                }

                public static IEnumerable<long> ByProject(IQuery query, IEnumerable<long> ids)
                {
                    return from project in query.For<Facts.Project>()
                           join firm in query.For<Facts.Firm>() on project.OrganizationUnitId equals firm.OrganizationUnitId
                           where ids.Contains(project.Id)
                           select firm.Id;
                }

                public static IEnumerable<long> ByCategoryGroup(IQuery query, IEnumerable<long> ids)
                {
                    return (from categoryGroup in query.For<Facts.CategoryGroup>()
                            join categoryOrganizationUnit in query.For<Facts.CategoryOrganizationUnit>() on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                            join categoryFirmAddress in query.For<Facts.CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                            join firmAddress in query.For<Facts.FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                            where ids.Contains(categoryGroup.Id)
                            select firmAddress.FirmId).Distinct();
                }
            }

            public static class Territory
            {
                public static IEnumerable<long> ByProject(IQuery query, IEnumerable<long> ids)
                {
                    return from project in query.For<Facts.Project>()
                           join territory in query.For<Facts.Territory>() on project.OrganizationUnitId equals territory.OrganizationUnitId
                           where ids.Contains(project.Id)
                           select territory.Id;
                }
            }

            public static class Project
            {
                public static IEnumerable<long> ByCategoryOrganizationUnit(IQuery query, IEnumerable<long> ids)
                {
                    return from categoryOrganizationUnit in query.For<Facts.CategoryOrganizationUnit>()
                           join project in query.For<Facts.Project>() on categoryOrganizationUnit.OrganizationUnitId equals project.OrganizationUnitId
                           where ids.Contains(categoryOrganizationUnit.Id)
                           select project.Id;
                }
            }
        }
    }
}