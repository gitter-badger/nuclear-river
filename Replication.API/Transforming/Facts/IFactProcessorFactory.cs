using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactProcessorFactory
    {
        IFactProcessor Create(IFactInfo factInfo, IQuery query);
    }
}