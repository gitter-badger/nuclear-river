using System;
using System.Collections.Generic;

using LinqToDB.Data;

using IsolationLevel = System.Data.IsolationLevel;

namespace NuClear.AdvancedSearch.Replication.Data
{
    public sealed class Linq2DbDataConnectionTransactionManager : ITransactionManager
    {
        private const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;
        private readonly IEnumerable<DataConnection> _connections;
        private bool _transactionIsActive;

        public Linq2DbDataConnectionTransactionManager(IEnumerable<DataConnection> connections)
        {
            _connections = connections;
            _transactionIsActive = false;
        }

        public void BeginTransaction()
        {
            if (_transactionIsActive)
            {
                throw new InvalidOperationException("Transaction have already been started");
            }

            foreach (var dataConnection in _connections)
            {
                dataConnection.BeginTransaction(DefaultIsolationLevel);
            }

            _transactionIsActive = true;
        }

        public void CommitTransaction()
        {
            if (!_transactionIsActive)
            {
                throw new InvalidOperationException("No active transaction");
            } 
            
            foreach (var dataConnection in _connections)
            {
                dataConnection.CommitTransaction();
            }

            _transactionIsActive = false;
        }

        public void RollbackTransaction()
        {
            if (!_transactionIsActive)
            {
                throw new InvalidOperationException("No active transaction");
            } 
            
            foreach (var dataConnection in _connections)
            {
                dataConnection.RollbackTransaction();
            }

            _transactionIsActive = false;
        }
    }
}
