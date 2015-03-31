using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB;
using LinqToDB.Expressions;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.Data
{
    internal static class DataReader
    {
        private static readonly MethodInfo ContainsMethodInfo = MemberHelper.MethodOf(() => Enumerable.Contains(default(IEnumerable<long>), default(long)));
        private static readonly MethodInfo WhereMethodInfo = MemberHelper.MethodOf(() => Enumerable.Where(default(IEnumerable<IIdentifiable>), default(Func<IIdentifiable, bool>))).GetGenericMethodDefinition();
        private static readonly MethodInfo ReadMethodInfo = MemberHelper.MethodOf(() => DataReader.Read<IIdentifiable>(default(IDataContext))).GetGenericMethodDefinition();
        
        private static readonly IDictionary<Type, Func<IDataContext, IEnumerable<long>, IEnumerable>> Selectors = new Dictionary<Type, Func<IDataContext, IEnumerable<long>, IEnumerable>>();

        private static readonly MethodInfo InsertMethodInfo = MemberHelper.MethodOf(() => DataExtensions.Insert(default(IDataContext), default(IIdentifiable))).GetGenericMethodDefinition();
        private static readonly MethodInfo UpdateMethodInfo = MemberHelper.MethodOf(() => DataExtensions.Update(default(IDataContext), default(IIdentifiable))).GetGenericMethodDefinition();

        public static IEnumerable<T> Read<T>(this IDataContext context, IEnumerable<long> ids) where T : class, IIdentifiable
        {
            return Read(context, typeof(T), ids).Cast<T>();
        }

        public static void Insert(this IDataContext context, Type type, IQueryable query)
        {
            foreach (var item in query)
            {
                InsertMethodInfo.MakeGenericMethod(type).Invoke(null, new[] { context, item });
            }
        }

        public static void Update(this IDataContext context, Type type, IQueryable query)
        {
            foreach (var item in query)
            {
                UpdateMethodInfo.MakeGenericMethod(type).Invoke(null, new[] { context, item });
            }
        }

        public static IEnumerable Read(this IDataContext context, Type type, IEnumerable<long> ids)
        {
            if (!typeof(IIdentifiable).IsAssignableFrom(type))
            {
                throw new ArgumentException("The type does not implement IEntity interface.", "type");
            }

            Func<IDataContext, IEnumerable<long>, IEnumerable> selector;
            if (!Selectors.TryGetValue(type, out selector))
            {
                var paramContext = Expression.Parameter(typeof(IDataContext), "context");
                var paramIds = Expression.Parameter(typeof(IEnumerable<long>), "ids");
                var paramEntity = Expression.Parameter(typeof(IIdentifiable), "x");

                var source = Expression.Call(null, ReadMethodInfo.MakeGenericMethod(type), Expression.Constant(context));
                var predicate = Expression.Lambda(
                    Expression.Call(null, ContainsMethodInfo, paramIds, Expression.Property(paramEntity, MemberHelper.PropertyOf<IIdentifiable>(x => x.Id))),
                    paramEntity);

                Selectors[type] =
                    selector =
                    Expression.Lambda<Func<IDataContext, IEnumerable<long>, IEnumerable>>(
                        Expression.Call(null, WhereMethodInfo.MakeGenericMethod(type), source, predicate),
                        paramContext,
                        paramIds).Compile();
            }

            return selector(context, ids);
        }

        private static IEnumerable<T> Read<T>(this IDataContext context) where T : class
        {
            return context.GetTable<T>();
        }
    }
}