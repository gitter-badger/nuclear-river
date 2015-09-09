using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactProcessor
    {
        IReadOnlyCollection<IOperation> ApplyChanges(IReadOnlyCollection<long> ids);
    }
}