using System;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public class UnityStatisticsFactImporterFactory : IStatisticsFactImporterFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IMetadataSource<IStatisticsFactInfo> _metadataSource;

        public UnityStatisticsFactImporterFactory(IUnityContainer unityContainer, IMetadataSource<IStatisticsFactInfo> metadataSource)
        {
            _unityContainer = unityContainer;
            _metadataSource = metadataSource;
        }

        public IStatisticsFactImporter Create(Type statisticsDtoType)
        {
            IStatisticsFactInfo statisticsFactInfo;
            if (!_metadataSource.Metadata.TryGetValue(statisticsDtoType, out statisticsFactInfo))
            {
                throw new ArgumentException("Specified statistics DTO type is not configured");
            }

            var importerType = typeof(StatisticsFactImporter<>).MakeGenericType(statisticsFactInfo.Type);
            var statisticsFactInfoType = typeof(IStatisticsFactInfo<>).MakeGenericType(statisticsFactInfo.Type);
            var processor = _unityContainer.Resolve(importerType, new DependencyOverride(statisticsFactInfoType, statisticsFactInfo));
            return (IStatisticsFactImporter)processor;
        }
    }
}