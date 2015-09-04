using System;

using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Readings;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class VerifiableFactProcessorFactory : IFactProcessorFactory
    {
        private readonly Action<IRepository> _onRepositoryCreated;

        public VerifiableFactProcessorFactory(Action<IRepository> onRepositoryCreated)
        {
            _onRepositoryCreated = onRepositoryCreated;
        }

        public IFactProcessor Create(IFactInfo factInfo, IQuery sourceQuery)
        {
            var repositoryType = typeof(IRepository<>).MakeGenericType(factInfo.Type);
            var repositoryInstance = (IRepository)DynamicMock.Of(repositoryType);

            _onRepositoryCreated(repositoryInstance);

            var applierType = typeof(FactProcessor<>).MakeGenericType(factInfo.Type);
            return (IFactProcessor)Activator.CreateInstance(applierType, factInfo, sourceQuery, repositoryInstance);
        }
    }
}