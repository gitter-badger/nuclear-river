using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = Model;
    using Facts = Model.Facts;
    
    public sealed class ErmFactsTransformationMetadata
    {
        // Правило по определению зависимых агрегатов: смотрим сборку CI сущностей из фактов (CustomerIntelligenceTransformationContext)
        // если видим join - считаем, что агрегат зависит от факта, если join'а нет - то нет (даже при наличии связи по Id)
        public static readonly Dictionary<Type, IFactInfo> Facts
            = new[]
              {
                  FactInfo.OfType<Facts.Account>()
                          .HasSource(Specs.Erm.Map.ToFacts.Accounts)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByAccount)
                          .Build(),

                  FactInfo.OfType<Facts.BranchOfficeOrganizationUnit>()
                          .HasSource(Specs.Erm.Map.ToFacts.BranchOfficeOrganizationUnits)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByBranchOfficeOrganizationUnit)
                          .Build(),

                  FactInfo.OfType<Facts.Category>()
                          .HasSource(Specs.Erm.Map.ToFacts.Categories)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByCategory)
                          .Build(),

                  FactInfo.OfType<Facts.CategoryFirmAddress>()
                          .HasSource(Specs.Erm.Map.ToFacts.CategoryFirmAddresses)
                          .LeadsToStatisticsCalculation(Specs.Facts.Map.ToStatistics.ByFirmAddressCategory)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByCategoryFirmAddress)
                          // .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByCategoryFirmAddressForStatistics)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByCategoryFirmAddress)
                          .Build(),

                  FactInfo.OfType<Facts.CategoryGroup>()
                          .HasSource(Specs.Erm.Map.ToFacts.CategoryGroups)
                          .HasMatchedAggregate<CI.CategoryGroup>()
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByCategoryGroup)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByCategoryGroup)
                          .Build(),

                  FactInfo.OfType<Facts.CategoryOrganizationUnit>()
                          .HasSource(Specs.Erm.Map.ToFacts.CategoryOrganizationUnits)
                          .HasDependentAggregate<CI.Project>(Specs.Facts.Map.ToProjectAggregate.ByCategoryOrganizationUnit)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByCategoryOrganizationUnit)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByCategoryOrganizationUnit)
                          .Build(),

                  FactInfo.OfType<Facts.Client>()
                          .HasSource(Specs.Erm.Map.ToFacts.Clients)
                          .HasMatchedAggregate<CI.Client>()
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByClient)
                          .Build(),

                  FactInfo.OfType<Facts.Contact>()
                          .HasSource(Specs.Erm.Map.ToFacts.Contacts)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByContacts)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByContacts)
                          .Build(),

                  FactInfo.OfType<Facts.Firm>()
                          .HasSource(Specs.Erm.Map.ToFacts.Firms)
                          .LeadsToStatisticsCalculation(Specs.Facts.Map.ToStatistics.ByFirm)
                          .HasMatchedAggregate<CI.Firm>()
                          // .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByFirmForStatistics)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByFirm)
                          .Build(),

                  FactInfo.OfType<Facts.FirmAddress>()
                          .HasSource(Specs.Erm.Map.ToFacts.FirmAddresses)
                          .LeadsToStatisticsCalculation(Specs.Facts.Map.ToStatistics.ByFirmAddress)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByFirmAddress)
                          // .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByFirmAddressForStatistics)
                          .HasDependentAggregate<CI.Client>(Specs.Facts.Map.ToClientAggregate.ByFirmAddress)
                          .Build(),

                  FactInfo.OfType<Facts.FirmContact>()
                          .HasSource(Specs.Erm.Map.ToFacts.FirmContacts)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByFirmContacts)
                          .Build(),

                  FactInfo.OfType<Facts.LegalPerson>()
                          .HasSource(Specs.Erm.Map.ToFacts.LegalPersons)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByLegalPerson)
                          .Build(),

                  FactInfo.OfType<Facts.Order>()
                          .HasSource(Specs.Erm.Map.ToFacts.Orders)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByOrder)
                          .Build(),

                  FactInfo.OfType<Facts.Project>()
                          .HasSource(Specs.Erm.Map.ToFacts.Projects)
                          .LeadsToStatisticsCalculation(Specs.Facts.Map.ToStatistics.ByProject)
                          .HasMatchedAggregate<CI.Project>()
                          .HasDependentAggregate<CI.Territory>(Specs.Facts.Map.ToTerritoryAggregate.ByProject)
                          .HasDependentAggregate<CI.Firm>(Specs.Facts.Map.ToFirmAggregate.ByProject)
                          .Build(),

                  FactInfo.OfType<Facts.Territory>()
                          .HasSource(Specs.Erm.Map.ToFacts.Territories)
                          .HasMatchedAggregate<CI.Territory>()
                          .Build(),

              }.ToDictionary(x => x.Type);
    }
}