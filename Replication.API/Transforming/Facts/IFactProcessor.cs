using System.Collections.Generic;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactProcessor
    {
        IReadOnlyCollection<IOperation> ApplyChanges(IQuery query, IDataChangesApplier applier, IReadOnlyCollection<long> ids);
    }
}