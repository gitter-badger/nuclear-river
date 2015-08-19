using System;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class VerifiableDataChangesApplierFactory : IDataChangesApplierFactory
    {
        private readonly Action<Type, IRepository> _onRepositoryCreated;

        public VerifiableDataChangesApplierFactory(Action<Type, IRepository> onRepositoryCreated)
        {
            _onRepositoryCreated = onRepositoryCreated;
        }

        public IDataChangesApplier Create(Type type)
        {
            var repositoryType = typeof(IRepository<>).MakeGenericType(type);
            var repositoryInstance = (IRepository)DynamicMock.Of(repositoryType);

            _onRepositoryCreated(type, repositoryInstance);

            var applierType = typeof(DataChangesApplier<>).MakeGenericType(type);
            return (IDataChangesApplier)Activator.CreateInstance(applierType, repositoryInstance);
        }
    }
}