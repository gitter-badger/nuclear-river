using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NuClear.Querying.Web.OData.DataAccess
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> GetById<T>(this IQueryable<T> query, long id)
        {
            var propertyInfo = typeof(T).GetProperty("Id");
            if (propertyInfo == null || propertyInfo.PropertyType != typeof(long))
            {
                throw new ArgumentException();
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            var constant = LinqExpressionHelper.ParametrizeConstant(id, typeof(long));
            var equals = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<TK> SelectManyProperties<T, TK>(this IQueryable<T> query, string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null || !typeof(IEnumerable<TK>).IsAssignableFrom(propertyInfo.PropertyType))
            {
                throw new ArgumentException();
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda<Func<T, IEnumerable<TK>>>(property, parameter);

            return query.SelectMany(lambda);
        }

        private static class LinqExpressionHelper
        {
            // linq constant parametrization required for better sql server perfomance
            public static Expression ParametrizeConstant(object value, Type type)
            {
                var parameter = Activator.CreateInstance(typeof(LinqParameter<>).MakeGenericType(type), value);

                return Expression.Field(Expression.Constant(parameter), "Value");
            }

            private sealed class LinqParameter<T>
            {
                // ReSharper disable once MemberCanBePrivate.Local
                public readonly T Value;

                public LinqParameter(T value)
                {
                    Value = value;
                }
            }
        }
    }
}