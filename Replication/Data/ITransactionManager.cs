namespace NuClear.AdvancedSearch.Replication.Data
{
    public interface ITransactionManager
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}