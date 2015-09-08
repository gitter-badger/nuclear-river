namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactProcessorFactory
    {
        IFactProcessor Create(IFactInfo factInfo);
    }
}