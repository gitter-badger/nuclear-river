namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public interface IStatisticsProcessorFactory
    {
        IStatisticsProcessor Create(IStatisticsInfo metadata);
    }
}