namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactDependencyProcessorFactory
    {
        IFactDependencyProcessor Create(IFactDependencyInfo metadata);
    }
}