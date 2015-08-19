using System;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.API.Transforming;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public class UnityDataChangesApplierFactory : IDataChangesApplierFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityDataChangesApplierFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IDataChangesApplier Create(Type type)
        {
            var applierType = typeof(IDataChangesApplier<>).MakeGenericType(type);
            return (IDataChangesApplier)_unityContainer.Resolve(applierType);
        }
    }
}