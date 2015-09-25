using System.Collections.Generic;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.Metadata;
using NuClear.Replication.Metadata.Aggregates;
using NuClear.Storage.Readings;

namespace NuClear.Replication.Core.Aggregates
{
    public class StatisticsProcessor<T> : IStatisticsProcessor 
        where T : class
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<T> _repository;
        private readonly StatisticsRecalculationMetadata<T> _metadata;
        private readonly DataChangesDetector<T, T> _changesDetector;

        public StatisticsProcessor(StatisticsRecalculationMetadata<T> metadata, IQuery query, IBulkRepository<T> repository)
        {
            _query = query;
            _repository = repository;
            _metadata = metadata;
            _changesDetector = new DataChangesDetector<T, T>(_metadata.MapSpecificationProviderForSource, _metadata.MapSpecificationProviderForTarget, _query);
        }

        public void RecalculateStatistics(long projectId, IReadOnlyCollection<long?> categoryIds)
        {
            var filter = _metadata.FindSpecificationProvider.Invoke(projectId, categoryIds);

            // —начала сравниением получаем различающиес€ записи,
            // затем получаем те из различающихс€, которые совпадают по идентификатору.
            var intermediateResult = _changesDetector.DetectChanges(Specs.Map.ZeroMapping<T>(), filter, _metadata.FieldComparer);
            var changes = MergeTool.Merge(intermediateResult.Difference, intermediateResult.Complement);

            // Ќаличие или отсутствие статистики - не повод создавать или удал€ть рубрики у фирм.
            // ѕоэтому только обновление.
            _repository.Update(changes.Intersection);
        }
    }
}