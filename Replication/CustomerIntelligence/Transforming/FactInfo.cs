using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal abstract class FactInfo
    {
        public static Builder<T> OfType<T>(params object[] x)
        {
            return new Builder<T>();
        }

        public abstract Type FactType { get; }

        public abstract Func<IFactsContext, IEnumerable<long>, IQueryable> Query { get; }

        public abstract IEnumerable<FactDependencyInfo> Aggregates { get; }

        internal class Builder<TFact>
        {
            private readonly List<FactDependencyInfo> _collection = new List<FactDependencyInfo>();
            private Func<IFactsContext, IEnumerable<long>, IQueryable<TFact>> _factProvider;

            public Builder<TFact> HasSource(Func<IFactsContext, IEnumerable<long>, IQueryable<TFact>> factQueryProvider)
            {
                _factProvider = factQueryProvider;
                return this;
            }

            public Builder<TFact> HasSource(Func<IFactsContext, IQueryable<TFact>> factQueryableProvider, Func<IQueryable<TFact>, IEnumerable<long>, IQueryable<TFact>> factFilterApplier)
            {
                _factProvider = (context, ids) =>
                                {
                                    var query = factQueryableProvider.Invoke(context);
                                    var filteredQuery = factFilterApplier.Invoke(query, ids);
                                    return filteredQuery;
                                };
                return this;
            }

            public Builder<TFact> HasDependentAggregate<TAggregate>(Func<IFactsContext, IEnumerable<long>, IEnumerable<long>> dependentAggregateIdsQueryProvider)
            {
                _collection.Add(FactDependencyInfo.Create<TAggregate>(dependentAggregateIdsQueryProvider));
                return this;
            }

            public Builder<TFact> HasMatchedAggregate<TAggregate>()
            {
                _collection.Add(FactDependencyInfo.Create<TAggregate>());
                return this;
            }

            public static implicit operator FactInfo(Builder<TFact> fact)
            {
                return new FactInfoImpl<TFact>(fact._factProvider, fact._collection);
            }
        }

        private class FactInfoImpl<T> : FactInfo
        {
            private readonly Func<IFactsContext, IEnumerable<long>, IQueryable> _query;
            private readonly IEnumerable<FactDependencyInfo> _aggregates;

            public FactInfoImpl(Func<IFactsContext, IEnumerable<long>, IQueryable> query, IEnumerable<FactDependencyInfo> aggregates)
            {
                _query = query;
                _aggregates = aggregates ?? Enumerable.Empty<FactDependencyInfo>();
            }

            public override Type FactType
            {
                get { return typeof(T); }
            }

            public override Func<IFactsContext, IEnumerable<long>, IQueryable> Query
            {
                get { return _query; }
            }

            public override IEnumerable<FactDependencyInfo> Aggregates
            {
                get { return _aggregates; }
            }
        }
    }
}