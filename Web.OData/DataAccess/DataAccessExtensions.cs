using System;
using System.Linq;
using System.Linq.Expressions;

namespace NuClear.AdvancedSearch.Web.OData.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IQueryable<T> GetById<T>(this IQueryable<T> query, long id)
        {
            var idPropertyInfo = typeof(T).GetProperty("Id");
            if (idPropertyInfo == null || idPropertyInfo.PropertyType != typeof(long))
            {
                throw new ArgumentException();
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var idProperty = Expression.Property(parameter, idPropertyInfo);
            var idConstant = Expression.Constant(id, typeof(long));
            var equals = Expression.Equal(idProperty, idConstant);
            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            return query.Where(lambda);
        }
    }
}