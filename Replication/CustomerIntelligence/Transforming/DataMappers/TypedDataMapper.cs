using System.Collections;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Data;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.DataMappers
{
    internal class TypedDataMapper<T> : ITypedDataMapper
    {
        private readonly IDataMapper _mapper;

        public TypedDataMapper(IDataMapper mapper)
        {
            _mapper = mapper;
        }

        public void Insert(IEnumerable items)
        {
            foreach (var item in items.Cast<T>())
            {
                _mapper.Insert(item);
            }
        }

        public void Update(IEnumerable items)
        {
            foreach (var item in items.Cast<T>())
            {
                _mapper.Update(item);
            }
        }

        public void Delete(IEnumerable items)
        {
            // TODO {m.pashuk, 15.07.2015}: убрать ToList когда буду разные connections для read и write
            foreach (var item in items.Cast<T>().ToList())
            {
                _mapper.Delete(item);
            }
        }
    }
}