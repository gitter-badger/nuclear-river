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
                    .HasSource(Specs.Facts.Map.ToCI.Firms)
                    .HasValueObject(Specs.Facts.Map.ToCI.FirmActivities, Specs.Find.CI.FirmActivities)
                    .HasValueObject(Specs.Facts.Map.ToCI.FirmBalances, Specs.Find.CI.FirmBalances)
                    .HasValueObject(Specs.Facts.Map.ToCI.FirmCategories, Specs.Find.CI.FirmCategories)
                    .Build(),

                AggregateOfType<Client>()
                    .HasSource(Specs.Facts.Map.ToCI.Clients)
                    .HasValueObject(Specs.Facts.Map.ToCI.ClientContacts, Specs.Find.CI.ClientContacts)
                    .Build(),

                AggregateOfType<Project>()
                    .HasSource(Specs.Facts.Map.ToCI.Projects)
                    .HasValueObject(Specs.Facts.Map.ToCI.ProjectCategories, Specs.Find.CI.ProjectCategories)
                    .Build(),

                AggregateOfType<Territory>()
                    .HasSource(Specs.Facts.Map.ToCI.Territories)
                    .Build(),

                AggregateOfType<CategoryGroup>()
                    .HasSource(Specs.Facts.Map.ToCI.CategoryGroups)
                    .Build()
            }.ToDictionary(x => x.Type);

        public static AggregateInfoBuilder<TAggregate> AggregateOfType<TAggregate>() 
            where TAggregate : class, ICustomerIntelligenceObject, IIdentifiable
        {
            return new AggregateInfoBuilder<TAggregate>();
        }
    }
}
