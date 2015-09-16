using System;
using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    // Облегчённая версия Metamodelling
    public interface IMetadataSource<T>
        where T: IMetadataInfo
    {
        IReadOnlyDictionary<Type, T> Metadata { get; }
    }
}