using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface ISourceChangesApplierFactory
    {
        ISourceChangesApplier Create(ErmFactInfo factInfo, IQuery sourceQuery, IQuery destQuery);
    }
}