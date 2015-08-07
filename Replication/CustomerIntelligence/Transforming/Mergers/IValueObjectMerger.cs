using System.Collections;

using NuClear.AdvancedSearch.Replication.API.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Mergers
{
    internal interface IValueObjectMerger
    {
        // TODO {m.pashuk, 15.07.2015}: предложить value objects дропать и создавать заново, вместо того чтобы мёрджить
        IMergeResult Merge(IEnumerable source, IEnumerable target);
    }
}