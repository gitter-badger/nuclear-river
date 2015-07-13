using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB.Expressions;

using NuClear.AdvancedSearch.Replication.Model;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.Data
{
    internal static class DataMapperExtensions
    {
        private static readonly MethodInfo InsertMethodInfo = ((MethodInfo)MemberHelper.GetMemeberInfo((Expression<Action<IDataMapper>>)(mapper => mapper.Insert(default(IIdentifiable))))).GetGenericMethodDefinition();
        private static readonly MethodInfo UpdateMethodInfo = ((MethodInfo)MemberHelper.GetMemeberInfo((Expression<Action<IDataMapper>>)(mapper => mapper.Update(default(IIdentifiable))))).GetGenericMethodDefinition();
        private static readonly MethodInfo DeleteMethodInfo = ((MethodInfo)MemberHelper.GetMemeberInfo((Expression<Action<IDataMapper>>)(mapper => mapper.Delete(default(IIdentifiable))))).GetGenericMethodDefinition();

        private static readonly ConcurrentDictionary<Type, MethodInfo> InsertMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static readonly ConcurrentDictionary<Type, MethodInfo> UpdateMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static readonly ConcurrentDictionary<Type, MethodInfo> DeleteMethods = new ConcurrentDictionary<Type, MethodInfo>();

        public static void InsertAll(this IDataMapper mapper, IQueryable query)
        {
            using (var probe = new Probe("Inserting " + query.ElementType.Name))
            {
                IEnumerable items;
                using (var p = new Probe("Querying"))
                    items = Enumerate(query);

                using (var p = new Probe("Insering"))
                    InvokeMethodOn(ResolveMethod(InsertMethods, InsertMethodInfo, query.ElementType), mapper, items);
            }
        }

        public static void UpdateAll(this IDataMapper mapper, IQueryable query)
        {
            using (var probe = new Probe("Updating " + query.ElementType.Name))
            {
                IEnumerable items;
                using (var p = new Probe("Querying"))
                    items = Enumerate(query);

                using (var p = new Probe("Updating"))
                    InvokeMethodOn(ResolveMethod(UpdateMethods, UpdateMethodInfo, query.ElementType), mapper, items);
            }
        }

        public static void DeleteAll(this IDataMapper mapper, IQueryable query)
        {
            // Перед удалением требуется полность вычитать результат запроса.
            // Возможно, это баг в linq2db: он использует единственный SqlCommand для всех запросов в течении жизни DataContext
            // Это является проблемой только при удалении, поскольку все остальные операции проводят чтение и запись через разные DataContext
            using (var probe = new Probe("Deleting " + query.ElementType.Name))
            {
                IEnumerable items;
                using (var p = new Probe("Querying"))
                    items = Enumerate(query);

                using (var p = new Probe("Deleting"))
                    InvokeMethodOn(ResolveMethod(DeleteMethods, DeleteMethodInfo, query.ElementType), mapper, items);
            }
        }

        private static void InvokeMethodOn(MethodInfo method, IDataMapper mapper, IEnumerable items)
        {
            foreach (var item in items)
            {
                method.Invoke(mapper, new[] { item });
            }
        }

        private static MethodInfo ResolveMethod(ConcurrentDictionary<Type, MethodInfo> methods, MethodInfo definition, Type type)
        {
            return methods.GetOrAdd(type, t => definition.MakeGenericMethod(t));
        }

        private static IEnumerable Enumerate(IQueryable queryable)
        {
            var e = queryable.GetEnumerator();
            var result = new List<object>();
            while (e.MoveNext())
            {
                result.Add(e.Current);
            }

            return result;
        }
    }
}