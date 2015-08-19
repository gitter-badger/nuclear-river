using System;
using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public delegate MapSpecification<IQuery, IEnumerable<long>> MapToDependentAggregateSpecProvider(IReadOnlyCollection<long> aggregateIds);

    public interface IFactDependencyInfo
    {
        Type AggregateType { get; }
        bool IsDirectDependency { get; }
        MapToDependentAggregateSpecProvider MapToDependentAggregateSpecProvider { get; } 
    }
}