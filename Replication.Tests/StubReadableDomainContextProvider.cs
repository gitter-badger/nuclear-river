using System;
using System.Transactions;

using LinqToDB;
using LinqToDB.Data;

using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class StubReadableDomainContextProvider : IReadableDomainContextProvider
    {
        private readonly IReadableDomainContext _readableDomainContext;

        public StubReadableDomainContextProvider(IDataContext dataContext)
        {
            _readableDomainContext = CreateReadableDomainContext(dataContext);
        }

        public IReadableDomainContext Get()
        {
            return _readableDomainContext;
        }

        private static IReadableDomainContext CreateReadableDomainContext(IDataContext dataContext)
        {
            return new LinqToDBDomainContext((DataConnection)dataContext, 
                                             new TransactionOptions
                                             {
                                                 IsolationLevel = IsolationLevel.ReadCommitted,
                                                 Timeout = TimeSpan.Zero
                                             },
                                             new NullPendingChangesHandlingStrategy());
        }
    }
}