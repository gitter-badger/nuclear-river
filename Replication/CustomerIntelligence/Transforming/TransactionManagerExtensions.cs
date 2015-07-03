using System;

using NuClear.AdvancedSearch.Replication.Data;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public static class TransactionManagerExtensions
    {
        public static TResult WithinTransaction<TResult>(this ITransactionManager manager, Func<TResult> function)
        {
            try
            {
                manager.BeginTransaction();
                var result = function.Invoke();
                manager.CommitTransaction();
                return result;
            }
            catch (Exception)
            {
                manager.RollbackTransaction();
                throw;
            }
        }

        public static void WithinTransaction(this ITransactionManager manager, Action function)
        {
            try
            {
                manager.BeginTransaction();
                function.Invoke();
                manager.CommitTransaction();
            }
            catch (Exception)
            {
                manager.RollbackTransaction();
                throw;
            }
        }
    }
}