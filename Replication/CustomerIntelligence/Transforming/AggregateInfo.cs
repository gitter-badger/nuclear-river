using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal abstract class AggregateInfo
    {
        public static Builder<TAggregate> OfType<TAggregate>()
            where TAggregate : ICustomerIntelligenceObject, IIdentifiableObject
        {
            return new Builder<TAggregate>();
        }

        public abstract Type AggregateType { get; }

        public abstract Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> Query { get; }

        public abstract IEnumerable<EntityInfo> Entities { get; }

        public abstract IEnumerable<ValueObjectInfo> ValueObjects { get; }

        public class Builder<TAggregate>
            where TAggregate : ICustomerIntelligenceObject, IIdentifiableObject
        {
            private Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> _aggregateProvider;
            private readonly List<EntityInfo> _entities;
            private readonly List<ValueObjectInfo> _valueObjects;

            public Builder()
            {
                _entities = new List<EntityInfo>();
                _valueObjects = new List<ValueObjectInfo>();
            }

            public static implicit operator AggregateInfo(Builder<TAggregate> builder)
            {
                return new AggregateInfoImpl<TAggregate>(builder._aggregateProvider, builder._entities, builder._valueObjects);
            }

            public Builder<TAggregate> HasSource(Func<ICustomerIntelligenceContext, IQueryable<TAggregate>> queryProvider)
            {
                _aggregateProvider = CreateFilteredQueryProvider(queryProvider, identifiable => identifiable.Id);
                return this;
            }

            public Builder<TAggregate> HasValueObject<TValueObject>(Func<ICustomerIntelligenceContext, IQueryable<TValueObject>> queryProvider, Expression<Func<TValueObject, long>> idSelector)
            {
                var filteredQueryProvider = CreateFilteredQueryProvider(queryProvider, idSelector);
                _valueObjects.Add(new ValueObjectInfo(filteredQueryProvider));
                return this;
            }

            public Builder<TAggregate> HasEntity<TEntity>(Func<ICustomerIntelligenceContext, IQueryable<TEntity>> queryProvider, Expression<Func<TEntity, long>> idSelector)
                where TEntity : IIdentifiableObject
            {
                var filteredQueryProvider = CreateFilteredQueryProvider(queryProvider, idSelector);
                _entities.Add(new EntityInfo(filteredQueryProvider));
                return this;
            }

            private static Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable<T>> CreateFilteredQueryProvider<T>(Func<ICustomerIntelligenceContext, IQueryable<T>> queryProvider, Expression<Func<T, long>> idSelector)
            {
                return (context, ids) =>
                {
                    var query = queryProvider.Invoke(context);
                    var filterExpression = CreateFilterExpression(ids, idSelector);
                    return query.Where(filterExpression);
                };
            }

            private static Expression<Func<T, bool>> CreateFilterExpression<T>(IEnumerable<long> ids, Expression<Func<T, long>> idSelector)
            {
                Expression<Func<T, bool>> example = foo => ids.Contains(0);
                var exampleMethodCall = (MethodCallExpression)example.Body;
                var methodCall = exampleMethodCall.Update(null, new[] { exampleMethodCall.Arguments[0], idSelector.Body });
                return Expression.Lambda<Func<T, bool>>(methodCall, idSelector.Parameters);
            }
        }

        private class AggregateInfoImpl<T> : AggregateInfo
        {
            private readonly Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> _query;
            private readonly IEnumerable<EntityInfo> _entities;
            private readonly IEnumerable<ValueObjectInfo> _valueObjects;

            public AggregateInfoImpl(
                Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> query,
                IEnumerable<EntityInfo> entities = null,
                IEnumerable<ValueObjectInfo> valueObjects = null)
            {
                _query = query;
                _entities = entities ?? Enumerable.Empty<EntityInfo>();
                _valueObjects = valueObjects ?? Enumerable.Empty<ValueObjectInfo>();
            }

            public override Type AggregateType
            {
                get { return typeof(T); }
            }

            public override Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> Query { get { return _query; } }

            public override IEnumerable<EntityInfo> Entities { get { return _entities; } }

            public override IEnumerable<ValueObjectInfo> ValueObjects { get { return _valueObjects; } }
        }
    }
}