namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public interface IValueObjectProcessorFactory
    {
        IValueObjectProcessor Create(IValueObjectInfo metadata);
    }
}