namespace NuClear.AdvancedSearch.Replication.Data
{
    public interface ITransactionsManager
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}