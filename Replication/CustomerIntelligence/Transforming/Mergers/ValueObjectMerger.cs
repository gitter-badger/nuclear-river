using System.Collections;
using System.Linq;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Mergers
{
    internal sealed class ValueObjectMerger<T> : IValueObjectMerger
    {
        MergeTool.IMergeResult IValueObjectMerger.Merge(IEnumerable source, IEnumerable target)
        {
            return MergeTool.Merge(source.Cast<T>(), target.Cast<T>());
        }
    }
}