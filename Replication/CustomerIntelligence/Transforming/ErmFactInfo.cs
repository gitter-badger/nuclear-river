using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal abstract class ErmFactInfo
    {
        public static Builder<TFact> OfType<TFact>(params object[] x)
            where TFact : IErmFactObject, IIdentifiable
        {
            return new Builder<TFact>();
        }

        public abstract Type FactType { get; }

        public abstract Func<IErmFactsContext, IEnumerable<long>, IQueryable> Query { get; }

        public abstract IEnumerable<FactDependencyInfo> Aggregates { get; }

        public abstract IEnumerable<AggregateOperation> ApplyTo(ErmFactsTransformation transformation, IReadOnlyCollection<long> ids);

        internal class Builder<TFact>
            where TFact : IErmFactObject, IIdentifiable
        {
            private readonly List<FactDependencyInfo> _dependencies = new List<FactDependencyInfo>();
            private Func<IErmFactsContext, IEnumerable<long>, IQueryable<TFact>> _query;

            public Builder<TFact> HasSource(Func<IErmFactsContext, IEnumerable<long>, IQueryable<TFact>> query)
            {
                _query = query;
                return this;
            }

            public Builder<TFact> HasSource(Func<IErmFactsContext, IQueryable<TFact>> factQueryableProvider)
            {
                _query = (context, ids) =>
                                {
                                    var query = factQueryableProvider.Invoke(context);
                                    var filteredQuery = query.Where(fact => ids.Contains(fact.Id));
                                    return filteredQuery;
                                };
                return this;
            }

            public Builder<TFact> HasDependentAggregate<TAggregate>(Func<IErmFactsContext, IEnumerable<long>, IEnumerable<long>> dependentAggregateIdsQueryProvider)
                where TAggregate : ICustomerIntelligenceObject
            {
                _dependencies.Add(FactDependencyInfo.Create<TAggregate>(dependentAggregateIdsQueryProvider));
                return this;
            }

            public Builder<TFact> HasMatchedAggregate<TAggregate>()
            {
                _dependencies.Add(FactDependencyInfo.Create<TAggregate>());
                return this;
            }

            public static implicit operator ErmFactInfo(Builder<TFact> builder)
            {
                return new ErmFactInfoImpl<TFact>(builder._query, builder._dependencies);
            }
        }

        private class ErmFactInfoImpl<T> : ErmFactInfo
            where T : IErmFactObject
        {
            private readonly Func<IErmFactsContext, IEnumerable<long>, IQueryable<T>> _query;
            private readonly IReadOnlyCollection<FactDependencyInfo> _aggregates;

            public ErmFactInfoImpl(Func<IErmFactsContext, IEnumerable<long>, IQueryable<T>> query, IReadOnlyCollection<FactDependencyInfo> aggregates)
            {
                _query = query;
                _aggregates = aggregates ?? new FactDependencyInfo[0];
            }

            public override Type FactType
            {
                get { return typeof(T); }
            }

            public override Func<IErmFactsContext, IEnumerable<long>, IQueryable> Query
            {
                get { return _query; }
            }

            public override IEnumerable<FactDependencyInfo> Aggregates
            {
                get { return _aggregates; }
            }

            public override IEnumerable<AggregateOperation> ApplyTo(ErmFactsTransformation transformation, IReadOnlyCollection<long> ids)
            {
                return transformation.Transform(_query, _aggregates, ids);
            }
        }
    }
}