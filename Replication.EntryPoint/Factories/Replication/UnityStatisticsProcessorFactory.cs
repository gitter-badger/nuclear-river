using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.API.Transforming.Statistics;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public class UnityStatisticsProcessorFactory : IStatisticsProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityStatisticsProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IStatisticsProcessor Create(IStatisticsInfo metadata)
        {
            var processorType = typeof(StatisticsProcessor<>).MakeGenericType(metadata.Type);
            var processor = _unityContainer.Resolve(processorType, new DependencyOverride(metadata.GetType(), metadata));
            return (IStatisticsProcessor)processor;
        }
    }
}