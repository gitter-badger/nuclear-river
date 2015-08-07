using System;
using System.Data;
using System.Transactions;

using LinqToDB.Data;

using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.LinqToDB.Connections;

using IsolationLevel = System.Transactions.IsolationLevel;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class StubModifiableDomainContextProvider : IModifiableDomainContextProvider
    {
        private readonly IModifiableDomainContext _modifiableDomainContext;

        public StubModifiableDomainContextProvider(IDbConnection connection, DataConnection dataContext)
        {
            _modifiableDomainContext = CreateModifiableDomainContext(connection, dataContext);
        }

        public IModifiableDomainContext Get<T>() where T : class
        {
            return _modifiableDomainContext;
        }

        private static IModifiableDomainContext CreateModifiableDomainContext(IDbConnection connection, DataConnection dataContext)
        {
            return new LinqToDBDomainContext(connection,
                                             dataContext,
                                             new NullIManagedConnectionStateScopeFactory(),
                                             new TransactionOptions
                                             {
                                                 IsolationLevel = IsolationLevel.ReadCommitted,
                                                 Timeout = TimeSpan.Zero
                                             },
                                             new NullPendingChangesHandlingStrategy());
        }
    }
}