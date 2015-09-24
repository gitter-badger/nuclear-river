using System;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.Facts;
using NuClear.Replication.Metadata;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public class UnityStatisticsImporterFactory : IStatisticsImporterFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IMetadataProvider _metadataProvider;
        
        public UnityStatisticsImporterFactory(IUnityContainer unityContainer, IMetadataProvider metadataProvider)
        {
            _unityContainer = unityContainer;
            _metadataProvider = metadataProvider;
        }

        public IStatisticsImporter Create(Type statisticsDtoType)
        {
            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<ImportStatisticsMetadataIdentity>(out metadataSet))
            {
                throw new NotSupportedException(string.Format("Metadata for identity '{0}' cannot be found.", typeof(ImportStatisticsMetadataIdentity).Name));
            }

            IMetadataElement importStatisticsMetadata;
            Uri factMetadataId = Metadata.Id.For(ImportStatisticsMetadataIdentity.Instance.Id, statisticsDtoType.Name);
            if (!metadataSet.Metadata.TryGetValue(factMetadataId, out importStatisticsMetadata))
            {
                throw new NotSupportedException(string.Format("Import for statistics of type '{0}' is not supported.", statisticsDtoType));
            }

            var statisticsType = importStatisticsMetadata.GetType().GenericTypeArguments[0];
            var importerType = typeof(StatisticsFactImporter<>).MakeGenericType(statisticsType);
            var processor = _unityContainer.Resolve(importerType, new DependencyOverride(importStatisticsMetadata.GetType(), importStatisticsMetadata));
            return (IStatisticsImporter)processor;
        }
    }
}