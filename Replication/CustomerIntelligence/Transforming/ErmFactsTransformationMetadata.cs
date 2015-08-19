using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = Model;

    public sealed class ErmFactsTransformationMetadata
    {
        // Правило по определению зависимых агрегатов: смотрим сборку CI сущностей из фактов (CustomerIntelligenceTransformationContext)
        // если видим join - считаем, что агрегат зависит от факта, если join'а нет - то нет (даже при наличии связи по Id)
        public static readonly Dictionary<Type, IFactInfo> Facts
            = new[]
              {
                  FactInfo.OfType<Account>()
                          .HasSource(Specs.Erm.Map.ToFacts.Accounts)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByAccount)
                          .Build(),

                  FactInfo.OfType<BranchOfficeOrganizationUnit>()
                          .HasSource(Specs.Erm.Map.ToFacts.BranchOfficeOrganizationUnits)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByBranchOfficeOrganizationUnit)
                          .Build(),

                  FactInfo.OfType<Category>()
                          .HasSource(Specs.Erm.Map.ToFacts.Categories)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByCategory)
                          .Build(),

                  FactInfo.OfType<CategoryFirmAddress>()
                          .HasSource(Specs.Erm.Map.ToFacts.CategoryFirmAddresses)
                          .LeadsToStatisticsCalculation(Specs.Facts.Map.ToStatistics.ByFirmAddressCategory)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByCategoryFirmAddress)
                          // .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByCategoryFirmAddressForStatistics)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByCategoryFirmAddress)
                          .Build(),

                  FactInfo.OfType<CategoryGroup>()
                          .HasSource(Specs.Erm.Map.ToFacts.CategoryGroups)
                          .HasMatchedAggregate<CI.CategoryGroup>()
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByCategoryGroup)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByCategoryGroup)
                          .Build(),

                  FactInfo.OfType<CategoryOrganizationUnit>()
                          .HasSource(Specs.Erm.Map.ToFacts.CategoryOrganizationUnits)
                          .HasDependentAggregate<CI.Project>(Specs.Facts.Map.ToProjectAggregate.ByCategoryOrganizationUnit)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByCategoryOrganizationUnit)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByCategoryOrganizationUnit)
                          .Build(),

                  FactInfo.OfType<Client>()
                          .HasSource(Specs.Erm.Map.ToFacts.Clients)
                          .HasMatchedAggregate<CI.Client>()
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByClient)
                          .Build(),

                  FactInfo.OfType<Contact>()
                          .HasSource(Specs.Erm.Map.ToFacts.Contacts)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByContacts)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByContacts)
                          .Build(),

                  FactInfo.OfType<Firm>()
                          .HasSource(Specs.Erm.Map.ToFacts.Firms)
                          .LeadsToStatisticsCalculation(Specs.Facts.Map.ToStatistics.ByFirm)
                          .HasMatchedAggregate<CI.Firm>()
                          // .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByFirmForStatistics)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByFirm)
                          .Build(),

                  FactInfo.OfType<FirmAddress>()
                          .HasSource(Specs.Erm.Map.ToFacts.FirmAddresses)
                          .LeadsToStatisticsCalculation(Specs.Facts.Map.ToStatistics.ByFirmAddress)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByFirmAddress)
                          // .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByFirmAddressForStatistics)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByFirmAddress)
                          .Build(),

                  FactInfo.OfType<FirmContact>()
                          .HasSource(Specs.Erm.Map.ToFacts.FirmContacts)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByFirmContacts)
                          .Build(),

                  FactInfo.OfType<LegalPerson>()
                          .HasSource(Specs.Erm.Map.ToFacts.LegalPersons)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByLegalPerson)
                          .Build(),

                  FactInfo.OfType<Order>()
                          .HasSource(Specs.Erm.Map.ToFacts.Orders)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByOrder)
                          .Build(),

                  FactInfo.OfType<Project>()
                          .HasSource(Specs.Erm.Map.ToFacts.Projects)
                          .LeadsToStatisticsCalculation(Specs.Facts.Map.ToStatistics.ByProject)
                          .HasMatchedAggregate<CI.Project>()
                          .HasDependentAggregate<CI.Territory>(Specs.Facts.Map.ToTerritoryAggregate.ByProject)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByProject)
                          .Build(),

                  FactInfo.OfType<Territory>()
                          .HasSource(Specs.Erm.Map.ToFacts.Territories)
                          .HasMatchedAggregate<CI.Territory>()
                          .Build(),

              }.ToDictionary(x => x.Type);
    }
}