using System;
using System.Collections.Generic;

using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public interface IStatisticsInfo : IMetadataInfo
    {
    }

    public interface IStatisticsInfo<T>
    {
        MapToObjectsSpecProvider<T, T> MapSpecificationProviderForSource { get; }
        MapToObjectsSpecProvider<T, T> MapSpecificationProviderForTarget { get; }

        Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> FindSpecificationProvider { get; }
    }
}