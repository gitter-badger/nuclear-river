using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API.Aggregates;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public class UnityStatisticsProcessorFactory : IStatisticsProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityStatisticsProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IStatisticsProcessor Create(IMetadataElement metadata)
        {
            var statisticsType = metadata.GetType().GenericTypeArguments[0];
            var processorType = typeof(StatisticsProcessor<>).MakeGenericType(statisticsType);
            var processor = _unityContainer.Resolve(processorType, new DependencyOverride(metadata.GetType(), metadata));
            return (IStatisticsProcessor)processor;
        }
    }
}