using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB.Expressions;

using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;
using NuClear.AdvancedSearch.Replication.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public abstract class BaseTransformation
    {
        private readonly IDataMapper _mapper;

        protected BaseTransformation(IDataMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException("mapper");
            }
            _mapper = mapper;
        }

        protected void Load(Operation operation, IQueryable query)
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

        #region Adapter

        private static class Adapter
        {
            private static readonly MethodInfo InsertMethodInfo = ((MethodInfo)MemberHelper.GetMemeberInfo((Expression<Action<IDataMapper>>)(mapper => mapper.Insert(default(IIdentifiableObject))))).GetGenericMethodDefinition();
            private static readonly MethodInfo UpdateMethodInfo = ((MethodInfo)MemberHelper.GetMemeberInfo((Expression<Action<IDataMapper>>)(mapper => mapper.Update(default(IIdentifiableObject))))).GetGenericMethodDefinition();
            private static readonly MethodInfo DeleteMethodInfo = ((MethodInfo)MemberHelper.GetMemeberInfo((Expression<Action<IDataMapper>>)(mapper => mapper.Delete(default(IIdentifiableObject))))).GetGenericMethodDefinition();

            private static readonly ConcurrentDictionary<Type, MethodInfo> InsertMethods = new ConcurrentDictionary<Type, MethodInfo>();
            private static readonly ConcurrentDictionary<Type, MethodInfo> UpdateMethods = new ConcurrentDictionary<Type, MethodInfo>();
            private static readonly ConcurrentDictionary<Type, MethodInfo> DeleteMethods = new ConcurrentDictionary<Type, MethodInfo>();

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

            private static MethodInfo ResolveMethod(ConcurrentDictionary<Type, MethodInfo> methods, MethodInfo definition, Type type)
            {
                return methods.GetOrAdd(type, t => definition.MakeGenericMethod(t));
            }
        }

        #endregion
    }
}