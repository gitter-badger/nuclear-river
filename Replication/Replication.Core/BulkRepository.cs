using System.Collections.Generic;

using NuClear.Replication.Core.API;
using NuClear.Storage.Writings;

namespace NuClear.Replication.Core
{
    public class BulkRepository<TTarget> : IBulkRepository<TTarget> 
        where TTarget : class
    {
        private readonly IRepository<TTarget> _repository;

        public BulkRepository(IRepository<TTarget> repository)
        {
            _repository = repository;
        }

        public void Create(IEnumerable<TTarget> objects)
        {
            _repository.AddRange(objects);
            _repository.Save();
        }

        public void Update(IEnumerable<TTarget> objects)
        {
            foreach (var obj in objects)
            {
                _repository.Update(obj);
            }

            _repository.Save();
        }

        public void Delete(IEnumerable<TTarget> objects)
        {
            _repository.DeleteRange(objects);
            _repository.Save();
        }
    }
}