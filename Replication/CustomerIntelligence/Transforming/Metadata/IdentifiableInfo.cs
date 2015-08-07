using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal class IdentifiableInfo<T> : IIdentifiableInfo
    {
        private readonly Func<IQuery, IEnumerable<long>, IQueryable<T>> _queryByIds;

        // сделать чтобы сюда приходил только queryprovider и самим тут накручитьвать всё что нужно
        public IdentifiableInfo(Func<IQuery, IEnumerable<long>, IQueryable<T>> queryByIds)
        {
            _queryByIds = queryByIds;
        }

        public Func<IQuery, IEnumerable<long>, IQueryable<T>> QueryByIds { get { return _queryByIds; } }

        public Type Type { get { return typeof(T); } }
        IEnumerable IIdentifiableInfo.QueryByIds(IQuery query, IReadOnlyCollection<long> ids)
        {
            if (!ids.Any())
            {
                return Enumerable.Empty<T>();
            }

            return QueryByIds(query, ids);
        }

        IEnumerable<long> IIdentifiableInfo.QueryIdsByIds(IQuery context, IReadOnlyCollection<long> ids)
        {
            if (!ids.Any())
            {
                return Enumerable.Empty<long>();
            }

            var actualIds = QueryByIds(context, ids).Select(SelectIdExpression());
            return actualIds;
        }

        protected static Expression<Func<T, long>> SelectIdExpression()
        {
            var parameterExpr = Expression.Parameter(typeof(T));
            var propertyInfo = typeof(T).GetProperty("Id");
            if (propertyInfo == null)
            {
                throw new ArgumentException();
            }
            var propertyExpr = Expression.Property(parameterExpr, propertyInfo);
            var lambdaExpr = Expression.Lambda<Func<T, long>>(propertyExpr, parameterExpr);

            return lambdaExpr;
        }
    }
}