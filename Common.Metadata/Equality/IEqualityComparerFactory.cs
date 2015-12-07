using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Common.Metadata.Equality
{
    public interface IEqualityComparerFactory
    {
        IEqualityComparer<T> CreateIdentityComparer<T>();
        IEqualityComparer<T> CreateCompleteComparer<T>();
    }
}
