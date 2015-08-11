using System;
using System.Collections;
using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    public delegate MapSpecification<IQuery, IEnumerable> MapToObjectsSpecProvider(IReadOnlyCollection<long> ids);

    internal interface IMetadataInfo
    {
        Type Type { get; }
        MapToObjectsSpecProvider MapToSourceSpecProvider { get; }
        MapToObjectsSpecProvider MapToTargetSpecProvider { get; }
    }
}