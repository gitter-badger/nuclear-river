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
        MapToObjectsSpecProvider<T, T> MapSpecificationProviderForSource { get; }
        MapToObjectsSpecProvider<T, T> MapSpecificationProviderForTarget { get; }

        Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; }
    }
}