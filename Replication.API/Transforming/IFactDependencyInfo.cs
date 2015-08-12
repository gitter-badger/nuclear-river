using System;
using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface IFactDependencyInfo
    {
        Type AggregateType { get; }
        bool IsDirectDependency { get; }
        Func<IEnumerable<long>, MapSpecification<IQuery, IEnumerable<long>>> DependentAggregateSpecProvider { get; } 
    }
}