using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public class StatisticsProcessor<T> : IStatisticsProcessor 
        where T : class
    {
        private readonly IQuery _query;
        private readonly IDataChangesApplier<T> _changesApplier;
        private readonly StatisticsInfo<T> _metadata;

        public StatisticsProcessor(StatisticsInfo<T> metadata, IQuery query, IDataChangesApplier<T> changesApplier)
        {
            _query = query;
            _changesApplier = changesApplier;
            _metadata = metadata;
        }

        public void RecalculateStatistics(long projectId, IReadOnlyCollection<long?> categoryIds)
        {
            var filter = _metadata.FindSpecificationProvider.Invoke(projectId, categoryIds);

            // —начала сравниением получаем различающиес€ записи,
            // затем получаем те из различающихс€, которые совпадают по идентификатору.
            var intermediateResult = MergeTool.Merge(
                _metadata.SourceMappingProvider.Invoke(filter).Map(_query).ToList(),
                _metadata.TargetMappingProvider.Invoke(filter).Map(_query).ToList(),
                _metadata.FieldComparer);

            var changes = MergeTool.Merge(intermediateResult.Difference, intermediateResult.Complement);

            // Ќаличие или отсутствие статистики - не повод создавать или удал€ть рубрики у фирм.
            // ѕоэтому только обновление.
            _changesApplier.Update(changes.Intersection.AsQueryable());
        }
    }
}