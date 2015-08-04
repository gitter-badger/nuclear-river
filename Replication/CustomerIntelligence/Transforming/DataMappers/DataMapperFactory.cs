using System;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata;
using NuClear.AdvancedSearch.Replication.Data;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.DataMappers
{
    internal static class DataMapperFactory
    {
        public static ITypedDataMapper CreateTypedDataMapper(IDataMapper mapper, IMetadataInfo metadataInfo)
        {
            var createType = typeof(TypedDataMapper<>).MakeGenericType(metadataInfo.Type);
            return (ITypedDataMapper)Activator.CreateInstance(createType, mapper);
        }
    }
}