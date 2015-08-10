using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal class IdentifiableInfo<T> : IIdentifiableInfo where T : class, IIdentifiable
    {
        private readonly Func<IEnumerable<long>, MapSpecification<IQuery, IQueryable<T>>> _mapToSourceSpecProvider;
        private readonly Func<IEnumerable<long>, MapSpecification<IQuery, IQueryable<T>>> _mapToTargetSpecProvider;

        // сделать чтобы сюда приходил только queryprovider и самим тут накручитьвать всё что нужно
        public IdentifiableInfo(
            Func<IEnumerable<long>, MapSpecification<IQuery, IQueryable<T>>> mapToSourceSpecProvider,
            Func<IEnumerable<long>, MapSpecification<IQuery, IQueryable<T>>> mapToTargetSpecProvider)
        {
            _mapToSourceSpecProvider = mapToSourceSpecProvider;
            _mapToTargetSpecProvider = mapToTargetSpecProvider;
        }

        public Type Type
        {
            get { return typeof(T); }
        }

        MapSpecification<IQuery, IEnumerable> IIdentifiableInfo.GetMapToSourceSpec(IReadOnlyCollection<long> ids)
        {
            if (!ids.Any())
            {
                return new MapSpecification<IQuery, IEnumerable>(q => Enumerable.Empty<T>());
            }

            var mapToSourceSpec = _mapToSourceSpecProvider(ids);
            return new MapSpecification<IQuery, IEnumerable>(mapToSourceSpec);
        }

        MapSpecification<IQuery, IEnumerable<long>> IIdentifiableInfo.GetMapToSourceIdsSpec(IReadOnlyCollection<long> ids)
        {
            if (!ids.Any())
            {
                return new MapSpecification<IQuery, IEnumerable<long>>(q => Enumerable.Empty<long>());
            }

            var mapToSourceSpec = _mapToSourceSpecProvider(ids);
            return new MapSpecification<IQuery, IEnumerable<long>>(q => mapToSourceSpec.Map(q).Select(x => x.Id));
        }

        MapSpecification<IQuery, IEnumerable> IIdentifiableInfo.GetMapToTargetSpec(IReadOnlyCollection<long> ids)
        {
            if (!ids.Any())
            {
                return new MapSpecification<IQuery, IEnumerable>(q => Enumerable.Empty<T>());
            }

            var mapToTargetSpec = _mapToTargetSpecProvider(ids);
            return new MapSpecification<IQuery, IEnumerable>(mapToTargetSpec);
        }

        MapSpecification<IQuery, IEnumerable<long>> IIdentifiableInfo.GetMapToTargetIdsSpec(IReadOnlyCollection<long> ids)
        {
            if (!ids.Any())
            {
                return new MapSpecification<IQuery, IEnumerable<long>>(q => Enumerable.Empty<long>());
            }

            var mapToTargetSpec = _mapToTargetSpecProvider(ids);
            return new MapSpecification<IQuery, IEnumerable<long>>(q => mapToTargetSpec.Map(q).Select(x => x.Id));
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