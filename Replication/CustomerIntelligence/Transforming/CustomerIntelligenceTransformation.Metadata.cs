using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class CustomerIntelligenceTransformation
    {
        private static readonly Dictionary<Type, AggregateInfo> Aggregates
            = new AggregateInfo[]
              {
                  AggregateInfo.OfType<Firm>()
                               .HasSource(context => context.Firms)
                               .HasValueObject(context => context.FirmBalances, x => x.FirmId)
                               .HasValueObject(context => context.FirmCategories, x => x.FirmId),

                  AggregateInfo.OfType<Client>()
                               .HasSource(context => context.Clients)
                               .HasEntity(context => context.Contacts, x => x.ClientId),

              }.ToDictionary(x => x.AggregateType);
    }
}
