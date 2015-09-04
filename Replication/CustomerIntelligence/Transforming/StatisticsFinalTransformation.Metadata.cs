using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Statistics;
using NuClear.AdvancedSearch.Replication.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

    public partial class StatisticsFinalTransformation
    {
        public static readonly IReadOnlyCollection<IStatisticsInfo> Statistics =
            new[]
            {
                StatisticsOfType<CI::FirmCategoryStatistics>()
                    .HasSource(Specs.Map.Statistics.StatisticsTransformationContext_FirmCategoryStatistics)
                    .HasTarget(Specs.Map.Statistics.StatisticsContext_FirmCategoryStatistics)
                    .HasFilter(
                        (projectId, categoryIds) =>
                        categoryIds.Contains(null)
                            ? Specs.Find.CI.FirmCategoryStatistics.ByProject(projectId)
                            : Specs.Find.CI.FirmCategoryStatistics.ByProjectAndCategories(projectId, categoryIds))
                    .Build()
            };

        private static StatisticsInfoBuilder<T> StatisticsOfType<T>()
        {
            return new StatisticsInfoBuilder<T>();
        }
    }
}
