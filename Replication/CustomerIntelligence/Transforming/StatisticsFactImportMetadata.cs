using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using Bit = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

    public class StatisticsFactImportMetadata : IMetadataSource<IStatisticsFactInfo>
    {
        private static readonly Dictionary<Type, IStatisticsFactInfo> Infos =
            new[]
            {
                StatisticsFactOfType<Bit::FirmCategoryStatistics>()
                    .HasSource<FirmStatisticsDto>(Specs.Map.Bit.FirmCategoryStatistics())
                    .Aggregated(Specs.Find.Bit.FirmCategoryStatistics.ByProject)
                    .Build(),

                StatisticsFactOfType<Bit::ProjectCategoryStatistics>()
                    .HasSource<CategoryStatisticsDto>(Specs.Map.Bit.ProjectCategoryStatistics())
                    .Aggregated(Specs.Find.Bit.ProjectCategoryStatistics.ByProject)
                    .Build()

            }.ToDictionary(x => x.Type);

        public IReadOnlyDictionary<Type, IStatisticsFactInfo> Metadata
        {
            get { return Infos; }
        }

        private static StatisticsFactBuilder<TStatisticsFact> StatisticsFactOfType<TStatisticsFact>() 
            where TStatisticsFact : class
        {
            return new StatisticsFactBuilder<TStatisticsFact>();
        }
    }
}