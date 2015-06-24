using System.Collections.Generic;

using LinqToDB.Data;

using IsolationLevel = System.Data.IsolationLevel;

namespace NuClear.AdvancedSearch.Replication.Data
{
    public sealed class Linq2DbDataConnectionTransactionsManager : ITransactionsManager
    {
        private const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;
        private readonly IEnumerable<DataConnection> _connections;

        public Linq2DbDataConnectionTransactionsManager(IEnumerable<DataConnection> connections)
        {
            _connections = connections;
        }

        public void BeginTransaction()
        {
            foreach (var dataConnection in _connections)
            {
                dataConnection.BeginTransaction(DefaultIsolationLevel);
            }
        }

        public void CommitTransaction()
        {
            foreach (var dataConnection in _connections)
            {
                dataConnection.CommitTransaction();
            }
        }

        public void RollbackTransaction()
        {
            foreach (var dataConnection in _connections)
            {
                dataConnection.RollbackTransaction();
            }
        }
    }
}
