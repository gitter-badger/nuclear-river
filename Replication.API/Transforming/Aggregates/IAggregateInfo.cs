using System;
using System.Collections.Generic;

using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public interface IAggregateInfo : IMetadataInfo
    {
        IReadOnlyCollection<IValueObjectInfo> ValueObjects { get; }
    }

    public interface IAggregateInfo<T>
    {
        MapToObjectsSpecProvider<T> SourceMappingSpecification { get; }
        MapToObjectsSpecProvider<T> TargetMappingSpecification { get; }

        Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; }
    }

}