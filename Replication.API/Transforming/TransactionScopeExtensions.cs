using System.Collections.Generic;
using System.Transactions;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public static class TransactionScopeExtensions
    {
        public static IEnumerable<T> AsUntransactional<T>(this IEnumerable<T> enumerable)
        {
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                foreach (var item in enumerable)
                {
                    yield return item;
                }
            }
        }
    }
}