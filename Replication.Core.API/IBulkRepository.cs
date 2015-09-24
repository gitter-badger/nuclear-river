using System.Collections.Generic;

namespace NuClear.Replication.Core.API
{
    public interface IBulkRepository<TTarget>
    {
        void Create(IEnumerable<TTarget> objects);
        void Update(IEnumerable<TTarget> objects);
        void Delete(IEnumerable<TTarget> objects);
    }
}