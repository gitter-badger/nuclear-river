using System;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface IDataChangesApplierFactory
    {
        IDataChangesApplier Create(Type type);
    }
}