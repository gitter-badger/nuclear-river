using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Common.Metadata.Model
{
    public interface IEqualityComparerFactory
    {
        IEqualityComparer<T> CreateIdentityComparer<T>();
        IEqualityComparer<T> CreateCompleteComparer<T>();
    }
}
