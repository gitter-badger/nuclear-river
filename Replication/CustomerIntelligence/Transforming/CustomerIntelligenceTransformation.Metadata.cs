using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.AdvancedSearch.Replication.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class CustomerIntelligenceTransformation
    {
        private static readonly Dictionary<Type, IAggregateInfo> Aggregates =
            new[]
            {
                AggregateOfType<Firm>()
                    .HasSource(Specs.Map.Facts.ToCI.Firms)
                    .HasValueObject(Specs.Map.Facts.ToCI.FirmActivities, Specs.Find.CI.FirmActivities)
                    .HasValueObject(Specs.Map.Facts.ToCI.FirmBalances, Specs.Find.CI.FirmBalances)
                    .HasValueObject(Specs.Map.Facts.ToCI.FirmCategories, Specs.Find.CI.FirmCategories)
                    .Build(),

                AggregateOfType<Client>()
                    .HasSource(Specs.Map.Facts.ToCI.Clients)
                    .HasValueObject(Specs.Map.Facts.ToCI.ClientContacts, Specs.Find.CI.ClientContacts)
                    .Build(),

                AggregateOfType<Project>()
                    .HasSource(Specs.Map.Facts.ToCI.Projects)
                    .HasValueObject(Specs.Map.Facts.ToCI.ProjectCategories, Specs.Find.CI.ProjectCategories)
                    .Build(),

                AggregateOfType<Territory>()
                    .HasSource(Specs.Map.Facts.ToCI.Territories)
                    .Build(),

                AggregateOfType<CategoryGroup>()
                    .HasSource(Specs.Map.Facts.ToCI.CategoryGroups)
                    .Build()
            }.ToDictionary(x => x.Type);

        public static AggregateInfoBuilder<TAggregate> AggregateOfType<TAggregate>() 
            where TAggregate : class, ICustomerIntelligenceObject, IIdentifiable
        {
            return new AggregateInfoBuilder<TAggregate>();
        }
    }
}
