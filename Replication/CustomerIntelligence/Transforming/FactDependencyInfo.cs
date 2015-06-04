using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal abstract class FactDependencyInfo
    {
        public static FactDependencyInfo Create<T>()
        {
            return new DirectAggregateDependencyInfo<T>();
        }

        public static FactDependencyInfo Create<T>(Func<IErmFactsContext, IEnumerable<long>, IEnumerable<long>> query)
        {
            return new AggregateDependencyInfo<T>(query);
        }

        public abstract Type AggregateType { get; }

        public abstract bool IsDirectDependency { get; }

        public abstract Func<IErmFactsContext, IEnumerable<long>, IEnumerable<long>> Query { get; }

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

            public override Func<IErmFactsContext, IEnumerable<long>, IEnumerable<long>> Query
            {
                get { return (ctx, ids) => ids; }
            }
        }

        private class AggregateDependencyInfo<T> : FactDependencyInfo
        {
            private readonly Func<IErmFactsContext, IEnumerable<long>, IEnumerable<long>> _query;

            public AggregateDependencyInfo(Func<IErmFactsContext, IEnumerable<long>, IEnumerable<long>> query)
            {
                _query = query;
            }

            public override Type AggregateType
            {
                get { return typeof(T); }
            }

            public override bool IsDirectDependency
            {
                get { return false; }
            }

            public override Func<IErmFactsContext, IEnumerable<long>, IEnumerable<long>> Query
            {
                get { return _query; }
            }
        }
    }
}