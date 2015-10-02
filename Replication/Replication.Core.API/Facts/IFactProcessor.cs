using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IFactProcessor
    {
        IReadOnlyCollection<IOperation> ApplyChanges(IReadOnlyCollection<long> ids);
    }
}