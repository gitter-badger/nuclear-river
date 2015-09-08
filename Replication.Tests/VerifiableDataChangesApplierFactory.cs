using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class VerifiableDataChangesApplierFactory : IDataChangesApplierFactory
    {
        private readonly Action<Type, IRepository> _onRepositoryCreated;
        private readonly Dictionary<Type, IRepository> _createdRepositories;

        public VerifiableDataChangesApplierFactory(Action<Type, IRepository> onRepositoryCreated)
        {
            _onRepositoryCreated = onRepositoryCreated;
            _createdRepositories = new Dictionary<Type, IRepository>();
        }

        public IRepository FindCreatedRepository(Type type)
        {
            IRepository repositoryInstance;
            _createdRepositories.TryGetValue(type, out repositoryInstance);
            return repositoryInstance;
        }

        public IDataChangesApplier Create(Type type)
        {
            IRepository repositoryInstance;
            if (!_createdRepositories.TryGetValue(type, out repositoryInstance))
            {
                var repositoryType = typeof(IRepository<>).MakeGenericType(type);
                repositoryInstance = (IRepository)DynamicMock.Of(repositoryType);
                _onRepositoryCreated(type, repositoryInstance);
                _createdRepositories.Add(type, repositoryInstance);
            }

            var applierType = typeof(DataChangesApplier<>).MakeGenericType(type);
            return (IDataChangesApplier)Activator.CreateInstance(applierType, repositoryInstance);
        }
    }
}