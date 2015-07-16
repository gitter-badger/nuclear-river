using System;
using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal abstract class FactDependencyInfo
    {
        public static FactDependencyInfo Create<T>()
        {
            return new DirectAggregateDependencyInfo<T>();
        }

        public static FactDependencyInfo Create<T>(Func<IQuery, IEnumerable<long>, IEnumerable<long>> query)
        {
            return new AggregateDependencyInfo<T>(query);
        }

        public static FactDependencyInfo Create<T>(Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> dependentAggregateSpecProvider)
        {
            return new AggregateDependencyInfo<T>(dependentAggregateSpecProvider);
        }

        public abstract Type AggregateType { get; }

        public abstract bool IsDirectDependency { get; }

        public abstract Func<IQuery, IEnumerable<long>, IEnumerable<long>> Query { get; }

        public abstract Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> DependentAggregateSpecProvider { get; }

        private class DirectAggregateDependencyInfo<T> : FactDependencyInfo
        {
            public override Type AggregateType
            {
                get { return typeof(T); }
            }

            public override bool IsDirectDependency
            {
                get { return true; }
            }

            public override Func<IQuery, IEnumerable<long>, IEnumerable<long>> Query
            {
                get { return (ctx, ids) => ids; }
            }

            public override Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> DependentAggregateSpecProvider
            {
                get { return ids => new MapSpecification<IQuery, IEnumerable<long>>(q => ids); }
            }
        }

        private class AggregateDependencyInfo<T> : FactDependencyInfo
        {
            private readonly Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> _dependentAggregateSpecProvider;
            private readonly Func<IQuery, IEnumerable<long>, IEnumerable<long>> _query;

            public AggregateDependencyInfo(Func<IQuery, IEnumerable<long>, IEnumerable<long>> query)
            {
                _query = query;
            }

            public AggregateDependencyInfo(Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> dependentAggregateSpecProvider)
            {
                _dependentAggregateSpecProvider = dependentAggregateSpecProvider;
            }

            public override Type AggregateType
            {
                get { return typeof(T); }
            }

            public override bool IsDirectDependency
            {
                get { return false; }
            }

            public override Func<IQuery, IEnumerable<long>, IEnumerable<long>> Query
            {
                get { return _query; }
            }

            public override Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> DependentAggregateSpecProvider
            {
                get { return _dependentAggregateSpecProvider; }
            }
        }
    }
}