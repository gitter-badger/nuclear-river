using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public class UnityFactProcessorFactory : IFactProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityFactProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IFactProcessor Create(IFactInfo factInfo)
        {
            var processorType = typeof(FactProcessor<>).MakeGenericType(factInfo.Type);
            var processor = _unityContainer.Resolve(processorType, new DependencyOverride(factInfo.GetType(), factInfo));
            return (IFactProcessor)processor;
        }
    }
}