using System.Collections;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Data;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.DataMappers
{
    internal sealed class ValueObjectDataMapper<T> : TypedDataMapper<T>, IValueObjectDataMapper
    {
        private readonly IDataMapper _mapper;

        public ValueObjectDataMapper(IDataMapper mapper): base(mapper)
        {
            _mapper = mapper;
        }

        MergeTool.IMergeResult IValueObjectDataMapper.Merge(IEnumerable items1, IEnumerable items2)
        {
            return MergeTool.Merge(items1.Cast<T>(), items2.Cast<T>());
        }
    }
}