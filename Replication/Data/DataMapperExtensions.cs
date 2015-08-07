using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB.Expressions;

using NuClear.AdvancedSearch.Replication.API.Model;
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

        public static void InsertAll<T>(this IDataMapper mapper, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                mapper.Insert(item);
            }
        }

        public static void InsertAll(this IDataMapper mapper, IQueryable query)
        {
            using (Probe.Create("Inserting", query.ElementType.Name))
            {
                InvokeMethodOn(ResolveMethod(InsertMethods, InsertMethodInfo, query.ElementType), mapper, query);
            }
        }

        public static void UpdateAll(this IDataMapper mapper, IQueryable query)
        {
            using (Probe.Create("Updating", query.ElementType.Name))
            {
                InvokeMethodOn(ResolveMethod(UpdateMethods, UpdateMethodInfo, query.ElementType), mapper, query);
            }
        }

        public static void DeleteAll(this IDataMapper mapper, IQueryable query)
        {
            // Перед удалением требуется полность вычитать результат запроса.
            // Возможно, это баг в linq2db: он использует единственный SqlCommand для всех запросов в течении жизни DataContext
            // Это является проблемой только при удалении, поскольку все остальные операции проводят чтение и запись через разные DataContext
            using (Probe.Create("Deleting", query.ElementType.Name))
            {
                var items = query.Cast<IObject>().ToArray();
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
    }
}