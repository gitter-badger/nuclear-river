using System;

using NuClear.Querying.Metadata;

namespace NuClear.Querying.EntityFramework.Building
{
    public interface ITypeProvider
    {
        Type Resolve(EntityElement entityElement);
    }
}