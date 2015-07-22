using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public partial class StatisticsPrimaryTransformation
    {
        private readonly IErmFactsContext _facts;

        public StatisticsPrimaryTransformation(IErmFactsContext facts)
        {
            _facts = facts;
        }

        public IEnumerable<StatisticsOperation> DetectStatisticsOperations(IEnumerable<FactOperation> enumerable)
        {
            var result = Enumerable.Empty<StatisticsOperation>();

            var ops = enumerable.GroupBy(x => x.FactType, x => x.FactId);
            foreach (var group in ops)
            {
                Query query;
                if (!Transformations.TryGetValue(group.Key, out query))
                {
                    continue;
                }

                result = result.Concat(query.Invoke(_facts, group).ToList());
            }

            return result;
        }
    }
}
