using System.Collections.Generic;
using System.Reflection;

namespace NuClear.AdvancedSearch.Common.Metadata.Model
{
    public interface IObjectPropertyProvider
    {
        IReadOnlyCollection<PropertyInfo> GetPrimaryKeyProperties<T>();
        IReadOnlyCollection<PropertyInfo> GetProperties<T>();
    }
}