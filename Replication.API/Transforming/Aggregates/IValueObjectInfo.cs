using System;
using System.Collections.Generic;

using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public interface IValueObjectInfo : IMetadataInfo
    {
    }

    public interface IValueObjectInfo<T>
    {
        MapToObjectsSpecProvider<T> SourceMappingProvider { get; }
        MapToObjectsSpecProvider<T> TargetMappingProvider { get; }

        Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; }
    }
}