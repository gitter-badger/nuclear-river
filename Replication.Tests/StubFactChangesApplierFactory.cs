using System;
using System.Data;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.Readings;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class StubFactChangesApplierFactory : IFactChangesApplierFactory
    {
        private readonly IDbConnection _connection;
        private readonly DataConnection _dataContext;

        public StubFactChangesApplierFactory(IDbConnection connection, DataConnection dataContext)
        {
            _connection = connection;
            _dataContext = dataContext;
        }

        public ISourceChangesApplier Create(IFactInfo factInfo, IQuery sourceQuery, IQuery destQuery)
        {
            var repositoryType = typeof(LinqToDBRepository<>).MakeGenericType(factInfo.Type);
            var repositoryInstance = (IRepository)Activator.CreateInstance(repositoryType, new StubModifiableDomainContextProvider(_connection, _dataContext));

            var applierType = typeof(FactChangesApplier<>).MakeGenericType(factInfo.Type);
            return (ISourceChangesApplier)Activator.CreateInstance(applierType, factInfo, sourceQuery, destQuery, repositoryInstance);
        }
    }
}