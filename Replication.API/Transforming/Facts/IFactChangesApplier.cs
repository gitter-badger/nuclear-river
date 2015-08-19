using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactChangesApplier
    {
        IReadOnlyCollection<AggregateOperation> ApplyChanges(IMergeResult<long> changes);
    }

    public interface IFactChangesApplier<TFact> : IFactChangesApplier where TFact : class, IErmFactObject
    {
    }
}