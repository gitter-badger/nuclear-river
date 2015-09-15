using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public class StatisticsProcessor<T> : IStatisticsProcessor 
        where T : class
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<T> _repository;
        private readonly StatisticsInfo<T> _metadata;

        public StatisticsProcessor(StatisticsInfo<T> metadata, IQuery query, IBulkRepository<T> repository)
        {
            _query = query;
            _repository = repository;
            _metadata = metadata;
        }

        public void RecalculateStatistics(long projectId, IReadOnlyCollection<long?> categoryIds)
        {
            var filter = _metadata.FindSpecificationProvider.Invoke(projectId, categoryIds);

            // —начала сравниением получаем различающиес€ записи,
            // затем получаем те из различающихс€, которые совпадают по идентификатору.
            var intermediateResult = MergeTool.Merge(
                _metadata.MapSpecificationProviderForSource.Invoke(filter).Map(_query).ToArray(),
                _metadata.MapSpecificationProviderForTarget.Invoke(filter).Map(_query).ToArray(),
                _metadata.FieldComparer);

            var changes = MergeTool.Merge(intermediateResult.Difference, intermediateResult.Complement);

            // Ќаличие или отсутствие статистики - не повод создавать или удал€ть рубрики у фирм.
            // ѕоэтому только обновление.
            _repository.Update(changes.Intersection);
        }
    }
}