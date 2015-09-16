using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public class UnityValueObjectProcessorFactory : IValueObjectProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityValueObjectProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IValueObjectProcessor Create(IValueObjectInfo metadata)
        {
            var processorType = typeof(ValueObjectProcessor<>).MakeGenericType(metadata.Type);
            var processor = _unityContainer.Resolve(processorType, new DependencyOverride(metadata.GetType(), metadata));
            return (IValueObjectProcessor)processor;
        }
    }
}