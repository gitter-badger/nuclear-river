using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal static class AggregateInfoBuilder
    {
        public static AggregateInfoBuilder<TAggregate> OfType<TAggregate>()
            where TAggregate : ICustomerIntelligenceObject, IIdentifiable
        {
            return new AggregateInfoBuilder<TAggregate>();
        }
    }

    internal class AggregateInfoBuilder<TAggregate>
        where TAggregate : ICustomerIntelligenceObject, IIdentifiable
    {
        private readonly List<IValueObjectInfo> _valueObjects;
        private Func<IQuery, IEnumerable<long>, IQueryable<TAggregate>> _queryByIds;

        public AggregateInfoBuilder()
        {
            _valueObjects = new List<IValueObjectInfo>();
        }

        public IAggregateInfo Build()
        {
            return new AggregateInfo<TAggregate>(_queryByIds, _valueObjects);
        }

        public AggregateInfoBuilder<TAggregate> HasSource(Func<IQuery, IQueryable<TAggregate>> queryProvider)
        {
            _queryByIds = CreateFilteredQueryProvider(queryProvider, CreateKeyAccessor<TAggregate>());
            return this;
        }

        public AggregateInfoBuilder<TAggregate> HasValueObject<TValueObject>(MapSpecification<IQuery, IQueryable<TValueObject>> queryProvider, Expression<Func<TValueObject, long>> parentIdSelector)
        {
            var queryByParentIds = CreateFilteredQueryProvider(queryProvider, parentIdSelector);
            _valueObjects.Add(new ValueObjectInfo<TValueObject>(queryByParentIds));
            return this;
        }

        private static Func<IQuery, IEnumerable<long>, IQueryable<T>> CreateFilteredQueryProvider<T>(Func<IQuery, IQueryable<T>> queryProvider, Expression<Func<T, long>> idSelector)
        {
            return (query, ids) =>
            {
                var queryable = queryProvider(query);
                var filterExpression = CreateFilterExpression(ids, idSelector);
                return queryable.Where(filterExpression);
            };
        }

        private static Expression<Func<T, long>> CreateKeyAccessor<T>()
        {
            // Если написать (TAggregate x) => x.Id, то в этом выражении свойство Id будет получено не у типа TAggregate, а у типа IIdentifiable
            // В большинстве случаев нам это пофиг, но вот когда имеем дело с linq2db - это становится важным, ибо остальной запрос построен 
            // вокруг типа TAggregate и он просто не знает, что такое IIdentifiable.Id
            var param = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, long>>(Expression.Property(param, "Id"), param);
        }

        private static Expression<Func<T, bool>> CreateFilterExpression<T>(IEnumerable<long> ids, Expression<Func<T, long>> idSelector)
        {
            Expression<Func<T, bool>> example = foo => ids.Contains(0);
            var exampleMethodCall = (MethodCallExpression)example.Body;
            var methodCall = exampleMethodCall.Update(null, new[] { exampleMethodCall.Arguments[0], idSelector.Body });
            return Expression.Lambda<Func<T, bool>>(methodCall, idSelector.Parameters);
        }
    }
}