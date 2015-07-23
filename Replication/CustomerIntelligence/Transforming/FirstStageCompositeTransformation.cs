using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class FirstStageCompositeTransformation
    {
        private readonly ErmFactsTransformation _ermFactsTransformation;
        private readonly StatisticsPrimaryTransformation _statisticsTransformation;

        public FirstStageCompositeTransformation(ErmFactsTransformation ermFactsTransformation, StatisticsPrimaryTransformation statisticsTransformation)
        {
            _ermFactsTransformation = ermFactsTransformation;
            _statisticsTransformation = statisticsTransformation;
        }

        public IReadOnlyCollection<IOperation> Transform(IReadOnlyCollection<FactOperation> operations)
        {
            var statisticsBeforeChanges = _statisticsTransformation.DetectStatisticsOperations(operations);
            var aggregates = _ermFactsTransformation.Transform(operations);
            var statisticsAfterChanges = _statisticsTransformation.DetectStatisticsOperations(operations);

            return Enumerable.Empty<IOperation>()
                             .Concat(statisticsBeforeChanges)
                             .Concat(aggregates)
                             .Concat(statisticsAfterChanges)
                             .Distinct()
                             .ToList();
        }
    }
}
