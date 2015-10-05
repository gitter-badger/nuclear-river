using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class CustomerIntelligenceTransformation
    {
        private static readonly Dictionary<Type, IAggregateInfo> Aggregates = new[]
              {
                  AggregateInfoBuilder.OfType<Firm>()
                               .HasSource(context => context.Firms)
                               .HasValueObject(context => context.FirmActivities, x => x.FirmId)
                               .HasValueObject(context => context.FirmBalances, x => x.FirmId)
                               .HasValueObject(context => context.FirmCategories, x => x.FirmId)
                               .HasValueObject(context => context.FirmTerritories, x => x.FirmId)
                               .Build(),

                  AggregateInfoBuilder.OfType<Client>()
                               .HasSource(context => context.Clients)
                               .HasValueObject(context => context.ClientContacts, x => x.ClientId)
                               .Build(),

                  AggregateInfoBuilder.OfType<Project>()
                               .HasSource(context => context.Projects)
                               .HasValueObject(context => context.ProjectCategories, x => x.ProjectId)
                               .Build(),

                  AggregateInfoBuilder.OfType<Territory>()
                               .HasSource(context => context.Territories)
                               .Build(),

                  AggregateInfoBuilder.OfType<CategoryGroup>()
                               .HasSource(context => context.CategoryGroups)
                               .Build(),

              }.ToDictionary(x => x.Type);
    }
}
