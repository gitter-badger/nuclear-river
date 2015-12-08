using System;

using NuClear.AdvancedSearch.Common.Metadata.Elements;

namespace NuClear.Querying.EntityFramework.Building
{
    public interface ITypeProvider
    {
        Type Resolve(EntityElement entityElement);
    }
}