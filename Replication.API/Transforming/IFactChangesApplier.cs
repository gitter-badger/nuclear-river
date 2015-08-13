using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface ISourceChangesApplier
    {
        IReadOnlyCollection<AggregateOperation> ApplyChanges(IMergeResult<long> changes);
    }

    public interface IFactChangesApplier<TFact> : ISourceChangesApplier where TFact : class, IErmFactObject
    {
    }
}