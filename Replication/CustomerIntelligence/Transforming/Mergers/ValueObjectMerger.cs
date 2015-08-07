using System.Collections;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Mergers
{
    internal sealed class ValueObjectMerger<T> : IValueObjectMerger
    {
        IMergeResult IValueObjectMerger.Merge(IEnumerable source, IEnumerable target)
        {
            return MergeTool.Merge(source.Cast<T>(), target.Cast<T>());
        }
    }
}