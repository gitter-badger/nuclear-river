using System;
using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public delegate MapSpecification<IQuery, IEnumerable<TOutput>> MapToObjectsSpecProvider<TFilter, TOutput>(FindSpecification<TFilter> specification);

    public interface IMetadataInfo
    {
        Type Type { get; }
    }
}