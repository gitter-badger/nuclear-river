using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal abstract class FactInfo
    {
        public static FactInfo Create<T>(Func<IFactsContext, IEnumerable<long>, IQueryable> query, params FactDependencyInfo[] dependences)
        {
            return new FactInfoImpl<T>(query, dependences);
        }

        public abstract Type FactType { get; }

        public abstract Func<IFactsContext, IEnumerable<long>, IQueryable> Query { get; }

        public abstract IEnumerable<FactDependencyInfo> Aggregates { get; }

        private class FactInfoImpl<T> : FactInfo
        {
            private readonly Func<IFactsContext, IEnumerable<long>, IQueryable> _query;
            private readonly IEnumerable<FactDependencyInfo> _aggregates;

            public FactInfoImpl(Func<IFactsContext, IEnumerable<long>, IQueryable> query, params FactDependencyInfo[] aggregates)
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