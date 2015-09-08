using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Statistics;
using NuClear.Storage.Readings;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public partial class StatisticsFinalTransformation
    {
        private readonly IQuery _query;
        private readonly IDataChangesApplierFactory _dataChangesApplierFactory;
        private readonly StatisticsProcessorFactory _statisticsProcessorFactory;

        public StatisticsFinalTransformation(IQuery query, IDataChangesApplierFactory dataChangesApplierFactory)
    {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (dataChangesApplierFactory == null)
        {
                throw new ArgumentNullException("dataChangesApplierFactory");
            }

            _query = query;
            _dataChangesApplierFactory = dataChangesApplierFactory;
            _statisticsProcessorFactory = new StatisticsProcessorFactory();
        }

        public void Recalculate(IEnumerable<CalculateStatisticsOperation> operations)
        {
            var metadata = Statistics.Single();
            var processor = _statisticsProcessorFactory.Create(metadata);

            using (Probe.Create("Recalculate Statistics Operations"))
            {
                foreach (var batch in operations.GroupBy(x => x.ProjectId, x => x.CategoryId))
                {
                    var changesApplier = _dataChangesApplierFactory.Create(metadata.Type);
                    processor.RecalculateStatistics(_query, changesApplier, batch.Key, batch.Distinct().ToList());
        }
            }
        }
    }
}
