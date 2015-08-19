using System;
using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public sealed class DirectAggregateDependencyInfo<TAggregate> : IFactDependencyInfo
    {
        public Type AggregateType
        {
            get { return typeof(TAggregate); }
        }

        public bool IsDirectDependency
        {
            get { return true; }
        }

        public MapToDependentAggregateSpecProvider MapToDependentAggregateSpecProvider
        {
            get { return ids => new MapSpecification<IQuery, IEnumerable<long>>(q => ids); }
        }
    }
}