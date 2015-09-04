namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public interface IAggregateProcessorFactory
    {
        IAggregateProcessor Create(IAggregateInfo factInfo);
    }
}