using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = Model;

    public sealed partial class ErmFactsTransformation
    {
        // Правило по определению зависимых агрегатов: смотрим сборку CI сущностей из фактов (CustomerIntelligenceTransformationContext)
        // если видим join - считаем, что агрегат зависит от факта, если join'а нет - то нет (даже при наличии связи по Id)
        public static readonly Dictionary<Type, IFactInfo> Facts
            = new[]
              {
                  FactOfType<Activity>()
                      .HasSource(Specs.Map.Erm.ToFacts.Activities)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByActivity)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByClientActivity)
                      .Build(),

                  FactOfType<Account>()
                      .HasSource(Specs.Map.Erm.ToFacts.Accounts)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByAccount)
                      .Build(),

                  FactOfType<BranchOfficeOrganizationUnit>()
                      .HasSource(Specs.Map.Erm.ToFacts.BranchOfficeOrganizationUnits)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByBranchOfficeOrganizationUnit)
                      .Build(),

                  FactOfType<Category>()
                      .HasSource(Specs.Map.Erm.ToFacts.Categories)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByCategory)
                      .Build(),

                  FactOfType<CategoryFirmAddress>()
                      .HasSource(Specs.Map.Erm.ToFacts.CategoryFirmAddresses)
                      .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByFirmAddressCategory)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByCategoryFirmAddress)
                      .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByCategoryFirmAddress)
                      .Build(),

                  FactOfType<CategoryGroup>()
                      .HasSource(Specs.Map.Erm.ToFacts.CategoryGroups)
                      .HasMatchedAggregate<CI::CategoryGroup>()
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByCategoryGroup)
                      .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByCategoryGroup)
                      .Build(),

                  FactOfType<CategoryOrganizationUnit>()
                      .HasSource(Specs.Map.Erm.ToFacts.CategoryOrganizationUnits)
                      .HasDependentAggregate<CI::Project>(Specs.Map.Facts.ToProjectAggregate.ByCategoryOrganizationUnit)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByCategoryOrganizationUnit)
                      .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByCategoryOrganizationUnit)
                      .Build(),

                  FactOfType<Client>()
                      .HasSource(Specs.Map.Erm.ToFacts.Clients)
                      .HasMatchedAggregate<CI::Client>()
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByClient)
                      .Build(),

                  FactOfType<Contact>()
                      .HasSource(Specs.Map.Erm.ToFacts.Contacts)
                      .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByContacts)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByContacts)
                      .Build(),

                  FactOfType<Firm>()
                      .HasSource(Specs.Map.Erm.ToFacts.Firms)
                      .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByFirm)
                      .HasMatchedAggregate<CI::Firm>()
                      .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByFirm)
                      .Build(),

                  FactOfType<FirmAddress>()
                      .HasSource(Specs.Map.Erm.ToFacts.FirmAddresses)
                      .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByFirmAddress)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByFirmAddress)
                      .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByFirmAddress)
                      .Build(),

                  FactOfType<FirmContact>()
                      .HasSource(Specs.Map.Erm.ToFacts.FirmContacts)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByFirmContacts)
                      .Build(),

                  FactOfType<LegalPerson>()
                      .HasSource(Specs.Map.Erm.ToFacts.LegalPersons)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByLegalPerson)
                      .Build(),

                  FactOfType<Order>()
                      .HasSource(Specs.Map.Erm.ToFacts.Orders)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByOrder)
                      .Build(),

                  FactOfType<Project>()
                      .HasSource(Specs.Map.Erm.ToFacts.Projects)
                      .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByProject)
                      .HasMatchedAggregate<CI::Project>()
                      .HasDependentAggregate<CI::Territory>(Specs.Map.Facts.ToTerritoryAggregate.ByProject)
                      .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByProject)
                      .Build(),

                  FactOfType<Territory>()
                      .HasSource(Specs.Map.Erm.ToFacts.Territories)
                      .HasMatchedAggregate<CI::Territory>()
                      .Build(),

              }.ToDictionary(x => x.Type);

        public static FactInfoBuilder<TFact> FactOfType<TFact>()
            where TFact : class, IErmFactObject, IIdentifiable
        {
            return new FactInfoBuilder<TFact>();
        }
    }
}