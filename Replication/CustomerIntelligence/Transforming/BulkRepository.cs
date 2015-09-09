using System.Collections;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public class BulkRepository<TTarget> : IBulkRepository<TTarget> 
        where TTarget : class
    {
        private readonly IRepository<TTarget> _repository;

        public BulkRepository(IRepository<TTarget> repository)
        {
            _repository = repository;
        }

        public void Create(IEnumerable objects)
        {
            _repository.AddRange(objects.Cast<TTarget>());
            _repository.Save();
        }

        public void Update(IEnumerable objects)
        {
            foreach (var obj in objects.Cast<TTarget>())
            {
                _repository.Update(obj);
            }

            _repository.Save();
        }

        public void Delete(IEnumerable objects)
        {
            _repository.DeleteRange(objects.Cast<TTarget>());
            _repository.Save();
        }
    }
}