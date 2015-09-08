using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

    public class StatisticsFinalTransformation
    {
        private readonly IStatisticsContext _source;
        private readonly IStatisticsContext _target;
        private readonly IDataMapper _mapper;

        public StatisticsFinalTransformation(IStatisticsContext source, IStatisticsContext target, IDataMapper mapper)
        {
            _source = source;
            _target = target;
            _mapper = mapper;
        }

        public void Recalculate(IEnumerable<CalculateStatisticsOperation> operations)
        {
            using (Probe.Create("Recalculate Statistics Operations"))
            {
                foreach (var project in operations.GroupBy(x => x.ProjectId, x => x.CategoryId))
                {
                    var filter = CreateFilter(project.Key, project.Distinct().ToList());
                    var changes = DetectChanges(filter);

                    // Наличие или отсутствие статистики - не повод создавать или удалять рубрики у фирм.
                    // Поэтому только обновление.
                    _mapper.UpdateAll(changes.Intersection.AsQueryable());
                }
            }
        }

        private static Expression<Func<CI.FirmCategoryStatistics, bool>> CreateFilter(long projectId, IReadOnlyCollection<long?> categoryIds)
        {
            if (categoryIds.Any(x => x == null))
            {
                return x => x.ProjectId == projectId;
            }

            return x => x.ProjectId == projectId && categoryIds.Contains(x.CategoryId);
        }

        private IMergeResult<CI.FirmCategoryStatistics> DetectChanges(Expression<Func<CI.FirmCategoryStatistics, bool>> filter)
        {
            // Сначала сравниением получаем различающиеся записи,
            // затем получаем те из различающихся, которые совпадают по идентификатору.
            var intermediateResult = MergeTool.Merge(
                _source.FirmCategoryStatistics.Where(filter).AsEnumerable(),
                _target.FirmCategoryStatistics.Where(filter).AsEnumerable(),
                new CI.FirmCategoryStatistics.FullEqualityComparer());

            return MergeTool.Merge(intermediateResult.Difference, intermediateResult.Complement);
        }
    }
}
