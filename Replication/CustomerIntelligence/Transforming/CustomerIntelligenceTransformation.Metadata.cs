using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata;
using NuClear.AdvancedSearch.Replication.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class CustomerIntelligenceTransformation
    {
        private static readonly Dictionary<Type, IAggregateInfo> Aggregates = new[]
              {
                  AggregateInfoBuilder.OfType<Firm>()
                               .HasSource(Specs.Facts.Map.ToCI.Firms)
                               .HasValueObject(Specs.Facts.Map.ToCI.FirmBalances, Specs.CI.Map.FirmBalances)
                               .HasValueObject(Specs.Facts.Map.ToCI.FirmCategories, Specs.CI.Map.FirmCategories)
                               .Build(),

                  AggregateInfoBuilder.OfType<Client>()
                               .HasSource(Specs.Facts.Map.ToCI.Clients)
                               .HasValueObject(Specs.Facts.Map.ToCI.ClientContacts, Specs.CI.Map.ClientContacts)
                               .Build(),

                  AggregateInfoBuilder.OfType<Project>()
                               .HasSource(Specs.Facts.Map.ToCI.Projects)
                               .HasValueObject(Specs.Facts.Map.ToCI.ProjectCategories, Specs.CI.Map.ProjectCategories)
                               .Build(),

                  AggregateInfoBuilder.OfType<Territory>()
                               .HasSource(Specs.Facts.Map.ToCI.Territories)
                               .Build(),

                  AggregateInfoBuilder.OfType<CategoryGroup>()
                               .HasSource(Specs.Facts.Map.ToCI.CategoryGroups)
                               .Build(),

              }.ToDictionary(x => x.Type);
    }
}
