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
        MapToObjectsSpecProvider<T, T> SourceMappingSpecification { get; }
        MapToObjectsSpecProvider<T, T> TargetMappingSpecification { get; }

        Func<long, IReadOnlyCollection<long?>, FindSpecification<T>> FindSpecificationProvider { get; }
    }
}