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
        private static readonly MethodInfo WhereMethodInfo = MemberHelper.MethodOf(() => Enumerable.Where(default(IEnumerable<IEntity>), default(Func<IEntity, bool>))).GetGenericMethodDefinition();
        private static readonly MethodInfo ReadMethodInfo = MemberHelper.MethodOf(() => DataReader.Read<IEntity>(default(IDataContext))).GetGenericMethodDefinition();
        
        private static readonly IDictionary<Type, Func<IDataContext, IEnumerable<long>, IEnumerable>> Selectors = new Dictionary<Type, Func<IDataContext, IEnumerable<long>, IEnumerable>>();

        public static IEnumerable<T> Read<T>(this IDataContext context, IEnumerable<long> ids) where T : class, IEntity
        {
            return Read(context, typeof(T), ids).Cast<T>();
        }

//        public static IEnumerable<EntityRecord<T>> Read<T>(this IDataContext context, IEnumerable<long> ids) where T : class, IEntity
//        {
//            //return Read(context, typeof(T), ids).Cast<T>();
//        }

        public static IEnumerable Read(this IDataContext context, Type type, IEnumerable<long> ids)
        {
            if (!typeof(IEntity).IsAssignableFrom(type))
            {
                throw new ArgumentException("The type does not implement IEntity interface.", "type");
            }

            Func<IDataContext, IEnumerable<long>, IEnumerable> selector;
            if (!Selectors.TryGetValue(type, out selector))
            {
                var paramContext = Expression.Parameter(typeof(IDataContext), "context");
                var paramIds = Expression.Parameter(typeof(IEnumerable<long>), "ids");
                var paramEntity = Expression.Parameter(typeof(IEntity), "x");

                var source = Expression.Call(null, ReadMethodInfo.MakeGenericMethod(type), Expression.Constant(context));
                var predicate = Expression.Lambda(
                    Expression.Call(null, ContainsMethodInfo, paramIds, Expression.Property(paramEntity, MemberHelper.PropertyOf<IEntity>(x => x.Id))),
                    paramEntity);

                Selectors[type] = selector = Expression.Lambda<Func<IDataContext, IEnumerable<long>, IEnumerable>>(
                    Expression.Call(null, WhereMethodInfo.MakeGenericMethod(type), source, predicate), paramContext, paramIds
                                                 ).Compile();
            }

            return selector(context, ids);
        }

        private static IEnumerable<T> Read<T>(this IDataContext context) where T : class
        {
            return context.GetTable<T>();
        }
    }

    internal class EntityRecord<T>
    {
        public long Id { get; set; }
        public T Entity { get; set; }
    }
}