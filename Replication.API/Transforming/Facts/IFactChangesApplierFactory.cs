using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactChangesApplierFactory
    {
        IFactChangesApplier Create(IFactInfo factInfo, IQuery query);
    }
}