using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface IBulkRepository<TTarget>
    {
        void Create(IEnumerable<TTarget> objects);
        void Update(IEnumerable<TTarget> objects);
        void Delete(IEnumerable<TTarget> objects);
    }
}