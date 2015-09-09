using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public sealed class UnityAggregateProcessorFactory : IAggregateProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityAggregateProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IAggregateProcessor Create(IAggregateInfo metadata)
        {
            var processorType = typeof(AggregateProcessor<>).MakeGenericType(metadata.Type);
            var processor = _unityContainer.Resolve(processorType, new DependencyOverride(metadata.GetType(), metadata));
            return (IAggregateProcessor)processor;
        }
    }
}