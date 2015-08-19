using System.Collections;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface IDataChangesApplier
    {
        void Create(IEnumerable objects);
        void Update(IEnumerable objects);
        void Delete(IEnumerable objects);
    }

    public interface IDataChangesApplier<TTarget> : IDataChangesApplier where TTarget : class 
    {
    }
}