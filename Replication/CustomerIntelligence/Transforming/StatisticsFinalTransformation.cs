using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Statistics;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public class StatisticsFinalTransformation
    {
        private readonly IMetadataSource<IStatisticsInfo> _metadataSource;
        private readonly IStatisticsProcessorFactory _statisticsProcessorFactory;

        public StatisticsFinalTransformation(IMetadataSource<IStatisticsInfo> metadataSource,
                                             IStatisticsProcessorFactory statisticsProcessorFactory)
        {
            if (metadataSource == null)
            {
                throw new ArgumentNullException("metadataSource");
            }

            if (statisticsProcessorFactory == null)
            {
                throw new ArgumentNullException("statisticsProcessorFactory");
            }

            _metadataSource = metadataSource;
            _statisticsProcessorFactory = statisticsProcessorFactory;
        }

        public void Recalculate(IEnumerable<CalculateStatisticsOperation> operations)
        {
            var metadata = _metadataSource.Metadata.Values.Single();
            var processor = _statisticsProcessorFactory.Create(metadata);

            using (Probe.Create("Recalculate Statistics Operations"))
            {
                foreach (var batch in operations.GroupBy(x => x.ProjectId, x => x.CategoryId))
                {
                    processor.RecalculateStatistics(batch.Key, batch.Distinct().ToArray());
                }
            }
        }
    }
}
