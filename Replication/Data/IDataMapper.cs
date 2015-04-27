namespace NuClear.AdvancedSearch.Replication.Data
{
    public interface IDataMapper
    {
        void Insert<T>(T item);
        
        void Update<T>(T item);
        
        void Delete<T>(T item);
    }
}