using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    internal class UnityFactDependencyProcessorFactory : IFactDependencyProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityFactDependencyProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IFactDependencyProcessor Create(IFactDependencyInfo metadata)
        {
            var processorType = typeof(FactDependencyProcessor<>).MakeGenericType(metadata.Type);
            var processor = _unityContainer.Resolve(processorType, new DependencyOverride(metadata.GetType(), metadata));
            return (IFactDependencyProcessor)processor;
        }
    }
}