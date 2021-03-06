﻿using System.Collections.Generic;
using System.Reflection;

namespace NuClear.AdvancedSearch.Common.Metadata.Equality
{
    public interface IObjectPropertyProvider
    {
        IReadOnlyCollection<PropertyInfo> GetPrimaryKeyProperties<T>();
        IReadOnlyCollection<PropertyInfo> GetProperties<T>();
    }
}