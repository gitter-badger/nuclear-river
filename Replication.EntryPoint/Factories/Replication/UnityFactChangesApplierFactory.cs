using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public class UnityFactProcessorFactory : IFactProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityFactProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IFactProcessor Create(IFactInfo factInfo, IQuery query)
        {
            var applierType = typeof(FactProcessor<>).MakeGenericType(factInfo.Type);
            return (IFactProcessor)_unityContainer.Resolve(
                applierType,
                new DependencyOverride(typeof(IFactInfo), factInfo),
                new DependencyOverride(typeof(IQuery), query));
        }
    }
}