using System;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.Tests.Data
{
    internal static class DataContextExtensions
    {
        public static IDataContext Has<T>(this IDataContext context, long id) where T : IIdentifiableObject, new()
        {
            return context.Has(Create<T>(id));
        }

        public static IDataContext Has<T>(this IDataContext context, T obj)
        {
            using (new NoSqlTrace())
            {
                context.Insert(obj);
                return context;
            }
        }

        public static T Read<T>(this IDataContext context, long id) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var predicate = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Property(parameter, "Id"), Expression.Constant(id)), parameter);

            return context.GetTable<T>().FirstOrDefault(predicate);
        }

        private static T Create<T>(long id) where T : IIdentifiableObject, new()
        {
            var obj = new T();
            obj.GetType().GetProperty("Id").SetValue(obj, id);
            return obj;
        }
    }
}