using System.Collections;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface IBulkRepository<TTarget>
    {
        void Create(IEnumerable objects);
        void Update(IEnumerable objects);
        void Delete(IEnumerable objects);
    }
}