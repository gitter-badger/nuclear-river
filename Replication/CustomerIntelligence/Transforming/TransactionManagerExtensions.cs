using System;

using NuClear.AdvancedSearch.Replication.Data;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public static class TransactionManagerExtensions
    {
        public static TResult InvokeTransactionalFunc<TArgument, TResult>(this ITransactionManager manager, Func<TArgument, TResult> function, TArgument argument)
        {
            try
            {
                manager.BeginTransaction();
                var result = function.Invoke(argument);
                manager.CommitTransaction();
                return result;
            }
            catch (Exception)
            {
                manager.RollbackTransaction();
                throw;
            }
        }

        public static void InvokeTransactionalAction<TArgument>(this ITransactionManager manager, Action<TArgument> function, TArgument argument)
        {
            try
            {
                manager.BeginTransaction();
                function.Invoke(argument);
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