using System;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.API.Transforming;
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

        public ISourceChangesApplier Create(IFactInfo factInfo, IQuery sourceQuery)
        {
            var repositoryType = typeof(IRepository<>).MakeGenericType(factInfo.Type);
            var repositoryInstance = (IRepository)DynamicMock(repositoryType);

            _onRepositoryCreated(repositoryInstance);

            var applierType = typeof(FactChangesApplier<>).MakeGenericType(factInfo.Type);
            return (ISourceChangesApplier)Activator.CreateInstance(applierType, factInfo, sourceQuery, repositoryInstance);
        }

        private static object DynamicMock(Type type)
        {
            var mock = typeof(Mock<>).MakeGenericType(type).GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            return mock.GetType().GetProperties().Single(f => f.Name == "Object" && f.PropertyType == type).GetValue(mock, new object[] { });
        }
    }
}