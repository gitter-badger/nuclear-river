using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IStatisticsFactInfo : IMetadataInfo
    {
    }

    public interface IStatisticsFactInfo<T> 
    {
        MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> MapSpecification { get; }
        Func<long, FindSpecification<T>> FindSpecificationProvider { get; }
    }
}