using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public class StatisticsProcessor<T> : IStatisticsProcessor
    {
        private readonly StatisticsInfo<T> _metadata;

        public StatisticsProcessor(StatisticsInfo<T> metadata)
        {
            _metadata = metadata;
        }

        public void RecalculateStatistics(IQuery query, IDataChangesApplier changesApplier, long projectId, IReadOnlyCollection<long?> categoryIds)
        {
            var filter = _metadata.FindSpecificationProvider.Invoke(projectId, categoryIds);

            // —начала сравниением получаем различающиес€ записи,
            // затем получаем те из различающихс€, которые совпадают по идентификатору.
            var intermediateResult = MergeTool.Merge(
                _metadata.SourceMappingSpecification.Invoke(filter).Map(query).ToList(),
                _metadata.TargetMappingSpecification.Invoke(filter).Map(query).ToList(),
                _metadata.FieldComparer);

            var changes = MergeTool.Merge(intermediateResult.Difference, intermediateResult.Complement);

            // Ќаличие или отсутствие статистики - не повод создавать или удал€ть рубрики у фирм.
            // ѕоэтому только обновление.
            changesApplier.Update(changes.Intersection.AsQueryable());
        }
    }
}