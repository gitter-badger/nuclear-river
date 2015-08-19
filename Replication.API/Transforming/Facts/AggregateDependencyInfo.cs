using System;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public sealed class AggregateDependencyInfo<TAggregate> : IFactDependencyInfo
    {
        private readonly MapToDependentAggregateSpecProvider _mapToDependentAggregateSpecProvider;

        public AggregateDependencyInfo(MapToDependentAggregateSpecProvider mapToDependentAggregateSpecProvider)
        {
            _mapToDependentAggregateSpecProvider = mapToDependentAggregateSpecProvider;
        }

        public Type AggregateType
        {
            get { return typeof(TAggregate); }
        }

        public bool IsDirectDependency
        {
            get { return false; }
        }

        public MapToDependentAggregateSpecProvider MapToDependentAggregateSpecProvider
        {
            get { return _mapToDependentAggregateSpecProvider; }
        }
    }
}