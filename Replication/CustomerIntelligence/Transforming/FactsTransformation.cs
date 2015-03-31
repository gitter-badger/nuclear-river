using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB.Expressions;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;
using NuClear.AdvancedSearch.Replication.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class FactsTransformation
    {
        private static readonly Dictionary<Type, Func<IFactsContext, IEnumerable<long>, IQueryable>> Queries = 
            new Dictionary<Type, Func<IFactsContext, IEnumerable<long>, IQueryable>>
                {
                    { typeof(Account), (context, ids) => context.Accounts.Where(x => ids.Contains(x.Id)) },
                    { typeof(CategoryFirmAddress), (context, ids) => context.CategoryFirmAddresses.Where(x => ids.Contains(x.Id)) },
                    { typeof(CategoryOrganizationUnit), (context, ids) => context.CategoryOrganizationUnits.Where(x => ids.Contains(x.Id)) },
                    { typeof(Client), (context, ids) => context.Clients.Where(x => ids.Contains(x.Id)) },
                    { typeof(Contact), (context, ids) => context.Contacts.Where(x => ids.Contains(x.Id)) },
                    { typeof(Firm), (context, ids) => context.Firms.Where(x => ids.Contains(x.Id)) },
                    { typeof(FirmAddress), (context, ids) => context.FirmAddresses.Where(x => ids.Contains(x.Id)) },
                    { typeof(FirmContact), (context, ids) => context.FirmContacts.Where(x => ids.Contains(x.Id)) },
                    { typeof(LegalPerson), (context, ids) => context.LegalPersons.Where(x => ids.Contains(x.Id)) },
                    { typeof(Order), (context, ids) => context.Orders.Where(x => ids.Contains(x.Id)) },
                };

        private readonly IFactsContext _source;
        private readonly IFactsContext _target;
        private readonly IDataMapper _mapper;

        public FactsTransformation(IFactsContext source, IFactsContext target, IDataMapper mapper)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (mapper == null)
            {
                throw new ArgumentNullException("mapper");
            }

            _source = source;
            _target = target;
            _mapper = mapper;
        }

        public IEnumerable<OperationInfo> Transform(IEnumerable<OperationInfo> operations)
        {
            var result = Enumerable.Empty<OperationInfo>();

            foreach (var slice in operations.GroupBy(x => new { x.EntityType, x.Operation }))
            {
                var operation = slice.Key.Operation;
                var entityType = slice.Key.EntityType;
                var entityIds = slice.Select(x => x.EntityId).ToArray();

                Func<IFactsContext, IEnumerable<long>, IQueryable> query;
                if (!Queries.TryGetValue(entityType, out query))
                {
                    // exception?
                    continue;
                }

                Load(operation, query(GetOperationContext(operation), entityIds));
            }

            return result;
        }

        private IFactsContext GetOperationContext(Operation operation)
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

        private void Load(Operation operation, IQueryable query)
        {
            switch (operation)
            {
                case Operation.Created:
                    Adapter.Insert(_mapper, query);
                    break;
                case Operation.Updated:
                    Adapter.Update(_mapper, query);
                    break;
                case Operation.Deleted:
                    Adapter.Delete(_mapper, query);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("operation");
            }
        }

        private static class Adapter
        {
            private static readonly MethodInfo InsertMethodInfo = ((MethodInfo)MemberHelper.GetMemeberInfo((Expression<Action<IDataMapper>>) (mapper => mapper.Insert(default(IIdentifiable))))).GetGenericMethodDefinition();
            private static readonly MethodInfo UpdateMethodInfo = ((MethodInfo)MemberHelper.GetMemeberInfo((Expression<Action<IDataMapper>>) (mapper => mapper.Update(default(IIdentifiable))))).GetGenericMethodDefinition();
            private static readonly MethodInfo DeleteMethodInfo = ((MethodInfo)MemberHelper.GetMemeberInfo((Expression<Action<IDataMapper>>) (mapper => mapper.Delete(default(IIdentifiable))))).GetGenericMethodDefinition();

            private static readonly ConcurrentDictionary<Type,MethodInfo> InsertMethods = new ConcurrentDictionary<Type, MethodInfo>();
            private static readonly ConcurrentDictionary<Type,MethodInfo> UpdateMethods = new ConcurrentDictionary<Type, MethodInfo>();
            private static readonly ConcurrentDictionary<Type,MethodInfo> DeleteMethods = new ConcurrentDictionary<Type, MethodInfo>();

            public static void Insert(IDataMapper mapper, IQueryable query)
            {
                InvokeMethodOn(ResolveMethod(InsertMethods, InsertMethodInfo, query.ElementType), mapper, query);
            }

            public static void Update(IDataMapper mapper, IQueryable query)
            {
                InvokeMethodOn(ResolveMethod(UpdateMethods, UpdateMethodInfo, query.ElementType), mapper, query);
            }

            public static void Delete(IDataMapper mapper, IQueryable query)
            {
                InvokeMethodOn(ResolveMethod(DeleteMethods, DeleteMethodInfo, query.ElementType), mapper, query);
            }

            private static void InvokeMethodOn(MethodInfo method, IDataMapper mapper, IEnumerable query)
            {
                foreach (var item in query)
                {
                    method.Invoke(mapper, new[] { item });
                }
            }

            private static MethodInfo ResolveMethod(ConcurrentDictionary<Type,MethodInfo> methods, MethodInfo definition, Type type)
            {
                return methods.GetOrAdd(type, t => definition.MakeGenericMethod(t));
            }
        }
    }
}
