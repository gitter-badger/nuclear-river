using System;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;

namespace NuClear.EntityDataModel.EntityFramework.Building
{
    public interface ITypeProvider
    {
        Type Resolve(EntityElement entityElement);
    }
}