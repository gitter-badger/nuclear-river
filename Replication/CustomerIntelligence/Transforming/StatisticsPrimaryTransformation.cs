using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Settings;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public partial class StatisticsPrimaryTransformation
    {
        private readonly IErmFactsContext _facts;
        private readonly IReplicationSettings _settings;

        public StatisticsPrimaryTransformation(IErmFactsContext facts, IReplicationSettings settings)
        {
            _facts = facts;
            _settings = settings;
        }

        public IEnumerable<CalculateStatisticsOperation> DetectStatisticsOperations(IEnumerable<FactOperation> factOperations)
        {
            using (Probe.Create("Detect Statistics Operations"))
            {
                var result = Enumerable.Empty<CalculateStatisticsOperation>();

                var groups = factOperations.GroupBy(x => x.FactType);

                foreach (var group in groups)
                {
                    Query query;
                    if (!Transformations.TryGetValue(group.Key, out query))
                    {
                        continue;
                    }

                    using (Probe.Create(group.Key.Name))
                    {
                        foreach (var batch in group.Select(x => x.FactId).Distinct().CreateBatches(_settings.ReplicationBatchSize))
                        {
                            var statisticsOperations = query.Invoke(_facts, batch);
                            result = result.Concat(statisticsOperations);
                        }
                    }
                }

                return result;
            }
        }
    }
}
