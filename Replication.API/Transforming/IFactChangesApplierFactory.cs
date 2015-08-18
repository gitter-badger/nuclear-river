using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface IFactChangesApplierFactory
    {
        ISourceChangesApplier Create(IFactInfo factInfo, IQuery query);
    }
}