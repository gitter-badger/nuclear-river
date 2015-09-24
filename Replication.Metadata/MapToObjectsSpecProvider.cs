using System.Collections.Generic;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.Replication.Metadata
{
    public delegate MapSpecification<IQuery, IEnumerable<TOutput>> MapToObjectsSpecProvider<TFilter, TOutput>(FindSpecification<TFilter> specification);
}