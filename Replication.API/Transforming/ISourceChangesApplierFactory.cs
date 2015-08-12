using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface ISourceChangesApplierFactory
    {
        ISourceChangesApplier Create(IFactInfo factInfo, IQuery source, IQuery target);
    }
}