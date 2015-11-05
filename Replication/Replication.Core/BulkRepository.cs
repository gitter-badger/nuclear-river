using System.Collections.Generic;

using NuClear.Replication.Core.API;
using NuClear.Storage.API.Writings;
using NuClear.Telemetry.Probing;

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
            using (Probe.Create("Inserting", typeof(TTarget).Name))
            {
                _repository.AddRange(objects);
                _repository.Save();
            }
        }

        public void Update(IEnumerable<TTarget> objects)
        {
            using (Probe.Create("Updating", typeof(TTarget).Name))
            {
                foreach (var obj in objects)
                {
                    _repository.Update(obj);
                }

                _repository.Save();
            }
        }

        public void Delete(IEnumerable<TTarget> objects)
        {
            using (Probe.Create("Deleting", typeof(TTarget).Name))
            {
                _repository.DeleteRange(objects);
                _repository.Save();
            }
        }
    }
}