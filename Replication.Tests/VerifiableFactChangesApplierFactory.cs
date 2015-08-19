using System;

using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Readings;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class VerifiableFactChangesApplierFactory : IFactChangesApplierFactory
    {
        private readonly Action<IRepository> _onRepositoryCreated;

        public VerifiableFactChangesApplierFactory(Action<IRepository> onRepositoryCreated)
        {
            _onRepositoryCreated = onRepositoryCreated;
        }

        public IFactChangesApplier Create(IFactInfo factInfo, IQuery sourceQuery)
        {
            var repositoryType = typeof(IRepository<>).MakeGenericType(factInfo.Type);
            var repositoryInstance = (IRepository)DynamicMock.Of(repositoryType);

            _onRepositoryCreated(repositoryInstance);

            var applierType = typeof(FactChangesApplier<>).MakeGenericType(factInfo.Type);
            return (IFactChangesApplier)Activator.CreateInstance(applierType, factInfo, sourceQuery, repositoryInstance);
        }
    }
}