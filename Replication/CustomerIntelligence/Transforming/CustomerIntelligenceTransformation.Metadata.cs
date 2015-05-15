using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using LinqToDB.Expressions;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class CustomerIntelligenceTransformation
    {
        private static readonly Dictionary<Type, AggregateInfo> Aggregates = new []
        {
            AggregateInfo.Create<Firm>(Find.Firms.ById, valueObjects:
                new[]
                {
                    new ValueObjectInfo(Find.FirmBalances.ByFirmId),
                    new ValueObjectInfo(Find.FirmCategories.ByFirmId),
                }),
            AggregateInfo.Create<Client>(Find.Clients.ById, new[] { new EntityInfo(Find.Contacts.ByClientIds) }),
        }.ToDictionary(x => x.AggregateType);

        private static class Find
        {
            public static class Clients
            {
                public static IQueryable<Client> ById(ICustomerIntelligenceContext context, IEnumerable<long> ids)
                {
                    return FilterById(context.Clients, ids);
                }
            }

            public static class Firms
            {
                public static IQueryable<Firm> ById(ICustomerIntelligenceContext context, IEnumerable<long> ids)
                {
                    return FilterById(context.Firms, ids);
                }
            }
                
            public static class FirmBalances
            {
                public static IQueryable<FirmBalance> ByFirmId(ICustomerIntelligenceContext context, IEnumerable<long> ids)
                {
                    return context.FirmBalances.Where(x => ids.Contains(x.FirmId));
                }
            }

            public static class FirmCategories
            {
                public static IQueryable<FirmCategory> ByFirmId(ICustomerIntelligenceContext context, IEnumerable<long> ids)
                {
                    return context.FirmCategories.Where(x => ids.Contains(x.FirmId));
                }
            }

            public static class Contacts
            {
                public static IQueryable<Contact> ByClientIds(ICustomerIntelligenceContext context, IEnumerable<long> ids)
                {
                    return context.Contacts.Where(x => ids.Contains(x.ClientId));
                }
            }

            private static IQueryable<TEntity> FilterById<TEntity>(IQueryable<TEntity> facts, IEnumerable<long> ids) where TEntity : IIdentifiableObject
            {
                return facts.Where(fact => ids.Contains(fact.Id));
            }
        }
    }
}
