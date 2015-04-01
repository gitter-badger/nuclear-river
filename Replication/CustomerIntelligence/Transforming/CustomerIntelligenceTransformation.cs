using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using LinqToDB.Expressions;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;
using NuClear.AdvancedSearch.Replication.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class CustomerIntelligenceTransformation : BaseTransformation
    {
        private static readonly Dictionary<Type, Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable>> Queries =
            new Dictionary<Type, Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable>>
                {
                    { typeof(Firm), (context, ids) => context.Firms.Where(x => ids.Contains(x.Id)) },
                    { typeof(Client), (context, ids) => context.Clients.Where(x => ids.Contains(x.Id)) },
                    { typeof(Contact), (context, ids) => context.Contacts.Where(x => ids.Contains(x.Id)) },
                };

        private static readonly Dictionary<Type, IEnumerable<Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable>>> RelatedQueries;

        static CustomerIntelligenceTransformation()
        {
            RelatedQueries = new Dictionary<Type, IEnumerable<Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable>>>
                             {
                                 { typeof(Firm), new List<Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable>>
                                                 {
                                                     (context, ids) => context.FirmAccounts.Where(x => ids.Contains(x.FirmId)),
                                                     (context, ids) => context.FirmCategories.Where(x => ids.Contains(x.FirmId))
                                                 } }
                             };
        }

        private readonly ICustomerIntelligenceContext _source;
        private readonly ICustomerIntelligenceContext _target;

        public CustomerIntelligenceTransformation(ICustomerIntelligenceContext source, ICustomerIntelligenceContext target, IDataMapper mapper)
            : base(mapper)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            _source = source;
            _target = target;
        }

        public IEnumerable<OperationInfo> Transform(IEnumerable<OperationInfo> operations)
        {
            var result = Enumerable.Empty<OperationInfo>();

            foreach (var slice in operations.GroupBy(x => new { x.EntityType, x.Operation }))
            {
                var operation = slice.Key.Operation;
                var entityType = slice.Key.EntityType;
                var entityIds = slice.Select(x => x.EntityId).ToArray();

                Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> query;
                if (!Queries.TryGetValue(entityType, out query))
                {
                    // exception?
                    continue;
                }

                IEnumerable<Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable>> relatedQueries;
                if (RelatedQueries.TryGetValue(entityType, out relatedQueries))
                {
                    foreach (var relatedQuery in relatedQueries)
                    {
                        var data = Utility.Merge(relatedQuery(_source, entityIds), relatedQuery(_target, entityIds));
                        var toDel = data.Item1;
                        var toAdd = data.Item2;

                        Load(Operation.Deleted, toDel.AsQueryable());
                        Load(Operation.Created, toAdd.AsQueryable());
                    }
                }

                Load(operation, query(GetOperationContext(operation), entityIds));
            }

            return result;
        }

        private ICustomerIntelligenceContext GetOperationContext(Operation operation)
        {
            switch (operation)
            {
                case Operation.Created:
                    return _source;
                case Operation.Updated:
                    return _source;
                case Operation.Deleted:
                    return _target;
                default:
                    throw new ArgumentOutOfRangeException("operation");
            }
        }

        #region Utility

        private static class Utility
        {
            private static readonly MethodInfo MergeMethodInfo = MemberHelper.MethodOf(() => Merge<IIdentifiable>(null, null)).GetGenericMethodDefinition();

            private static readonly ConcurrentDictionary<Type, MethodInfo> Methods = new ConcurrentDictionary<Type, MethodInfo>();

            public static Tuple<IEnumerable, IEnumerable> Merge(IQueryable newData, IQueryable oldData)
            {
                if (newData.ElementType != oldData.ElementType)
                {
                    throw new InvalidOperationException("The types are not matched.");
                }

                var type = newData.ElementType;
                var method = Methods.GetOrAdd(type, t => MergeMethodInfo.MakeGenericMethod(t));

                return (Tuple<IEnumerable, IEnumerable>)method.Invoke(null, new object[] { newData, oldData });
            }

            private static Tuple<IEnumerable, IEnumerable> Merge<T>(IEnumerable<T> newSetData, IEnumerable<T> oldSetData)
            {
                var newSet = new HashSet<T>(newSetData);
                var oldSet = new HashSet<T>(oldSetData);
                var toDel = oldSet.Where(x => !newSet.Contains(x));
                var toAdd = newSet.Where(x => !oldSet.Contains(x));

                return Tuple.Create<IEnumerable, IEnumerable>(toDel, toAdd);
            }

            #endregion
        }
    }
}
