using System.Collections;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.DataMappers
{
    internal interface ITypedDataMapper
    {
        void Insert(IEnumerable items);
        void Update(IEnumerable items);
        void Delete(IEnumerable items);
    }
}