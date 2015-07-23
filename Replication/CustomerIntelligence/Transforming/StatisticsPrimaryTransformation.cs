using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public partial class StatisticsPrimaryTransformation
    {
        private readonly IErmFactsContext _facts;

        public StatisticsPrimaryTransformation(IErmFactsContext facts)
        {
            _facts = facts;
        }

        public IEnumerable<StatisticsOperation> DetectStatisticsOperations(IEnumerable<FactOperation> factOperations)
        {
            using (Probe.Create("Detect Statistics Operations"))
            {
                var result = Enumerable.Empty<StatisticsOperation>();

                var ops = factOperations.GroupBy(x => x.FactType, x => x.FactId);
                foreach (var group in ops)
                {
                    Query query;
                    if (!Transformations.TryGetValue(group.Key, out query))
                    {
                        continue;
                    }

                    using (Probe.Create(group.Key.Name))
                    {
                        result = result.Concat(query.Invoke(_facts, group).ToList());
                    }
                }

                return result;
            }
        }
    }
}
