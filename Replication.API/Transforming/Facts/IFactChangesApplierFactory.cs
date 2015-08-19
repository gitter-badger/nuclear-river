using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactChangesApplierFactory
    {
        ISourceChangesApplier Create(IFactInfo factInfo, IQuery query);
    }
}