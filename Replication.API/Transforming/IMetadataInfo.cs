using System;
using System.Collections;
using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public delegate MapSpecification<IQuery, IEnumerable> MapToObjectsSpecProvider(IReadOnlyCollection<long> ids);

    public interface IMetadataInfo
    {
        Type Type { get; }
        MapToObjectsSpecProvider MapToSourceSpecProvider { get; }
        MapToObjectsSpecProvider MapToTargetSpecProvider { get; }
    }
}