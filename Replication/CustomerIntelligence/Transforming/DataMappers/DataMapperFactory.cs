using System;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata;
using NuClear.AdvancedSearch.Replication.Data;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.DataMappers
{
    internal static class DataMapperFactory
    {
        public static IValueObjectDataMapper CreateValueObjectDataMapper(IDataMapper mapper, IValueObjectInfo valueObjectInfo)
        {
            var createType = typeof(ValueObjectDataMapper<>).MakeGenericType(valueObjectInfo.Type);
            return (IValueObjectDataMapper)Activator.CreateInstance(createType, mapper);
        }

        public static ITypedDataMapper CreateTypedDataMapper(IDataMapper mapper, IIdentifiableInfo identifiableInfo)
        {
            var createType = typeof(TypedDataMapper<>).MakeGenericType(identifiableInfo.Type);
            return (ITypedDataMapper)Activator.CreateInstance(createType, mapper);
        }
    }
}