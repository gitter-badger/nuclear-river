using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Statistics;
using NuClear.AdvancedSearch.Replication.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

    public class StatisticsFinalTransformationMetadata : IMetadataSource<IStatisticsInfo>
    {
        public static readonly Dictionary<Type, IStatisticsInfo> Statistics =
            new[]
            {
                StatisticsOfType<CI::FirmCategoryStatistics>()
                    .HasSource(Specs.Map.Facts.ToStatistics.FirmCategoryStatistics)
                    .HasTarget(Specs.Map.CI.ToStatistics.FirmCategoryStatistics)
                    .HasFilter(
                        (projectId, categoryIds) =>
                        categoryIds.Contains(null)
                            ? Specs.Find.CI.FirmCategoryStatistics.ByProject(projectId)
                            : Specs.Find.CI.FirmCategoryStatistics.ByProjectAndCategories(projectId, categoryIds))
                    .HasFieldComparer(new CI::FirmCategoryStatistics.FullEqualityComparer())
                    .Build()
            }.ToDictionary(x => x.Type);

        public IReadOnlyDictionary<Type, IStatisticsInfo> Metadata
        {
            get { return Statistics; }
        }

        private static StatisticsInfoBuilder<T> StatisticsOfType<T>()
        {
            return new StatisticsInfoBuilder<T>();
        }
    }
}
