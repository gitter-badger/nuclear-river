using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public class UnityFactChangesApplierFactory : IFactChangesApplierFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityFactChangesApplierFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public ISourceChangesApplier Create(IFactInfo factInfo, IQuery sourceQuery, IQuery destQuery)
        {
            var applierType = typeof(IFactChangesApplier<>).MakeGenericType(factInfo.Type);
            return (ISourceChangesApplier)_unityContainer.Resolve(
                applierType,
                new DependencyOverride(typeof(IFactInfo), factInfo),
                new ParameterOverride("sourceQuery", sourceQuery),
                new ParameterOverride("destQuery", destQuery));
        }
    }
}