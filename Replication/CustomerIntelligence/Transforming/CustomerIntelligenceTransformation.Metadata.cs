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
                               .HasSource(query => query.For<Firm>())
                               .HasValueObject(Specs.Facts.Map.ToCI.FirmBalances(), x => x.FirmId)
                               .HasValueObject(Specs.Facts.Map.ToCI.FirmCategories(), x => x.FirmId)
                               .Build(),

                  AggregateInfoBuilder.OfType<Client>()
                               .HasSource(query => query.For<Client>())
                               .HasValueObject(Specs.Facts.Map.ToCI.ClientContacts(), x => x.ClientId)
                               .Build(),

                  AggregateInfoBuilder.OfType<Project>()
                               .HasSource(query => query.For<Project>())
                               .HasValueObject(Specs.Facts.Map.ToCI.ProjectCategories(), x => x.ProjectId)
                               .Build(),

                  AggregateInfoBuilder.OfType<Territory>()
                               .HasSource(query => query.For<Territory>())
                               .Build(),

                  AggregateInfoBuilder.OfType<CategoryGroup>()
                               .HasSource(query => query.For<CategoryGroup>())
                               .Build(),

              }.ToDictionary(x => x.Type);
    }
}
