using System;
using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public abstract class FactDependencyInfo
    {
        public abstract Type AggregateType { get; }

        public abstract bool IsDirectDependency { get; }

        public abstract Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> DependentAggregateSpecProvider { get; }

        public static FactDependencyInfo Create<TAggregate>()
        {
            return new DirectAggregateDependencyInfo<TAggregate>();
        }

        public static FactDependencyInfo Create<TAggregate>(Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> dependentAggregateSpecProvider)
        {
            return new AggregateDependencyInfo<TAggregate>(dependentAggregateSpecProvider);
        }

        private class DirectAggregateDependencyInfo<TAggregate> : FactDependencyInfo
        {
            public override Type AggregateType
            {
                get { return typeof(TAggregate); }
            }

            public override bool IsDirectDependency
            {
                get { return true; }
            }

            public override Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> DependentAggregateSpecProvider
            {
                get { return ids => new MapSpecification<IQuery, IEnumerable<long>>(q => ids); }
            }
        }

        private class AggregateDependencyInfo<TAggregate> : FactDependencyInfo
        {
            private readonly Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> _dependentAggregateSpecProvider;

            public AggregateDependencyInfo(Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> dependentAggregateSpecProvider)
            {
                _dependentAggregateSpecProvider = dependentAggregateSpecProvider;
            }

            public override Type AggregateType
            {
                get { return typeof(TAggregate); }
            }

            public override bool IsDirectDependency
            {
                get { return false; }
            }

            public override Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> DependentAggregateSpecProvider
            {
                get { return _dependentAggregateSpecProvider; }
            }
        }
    }
}