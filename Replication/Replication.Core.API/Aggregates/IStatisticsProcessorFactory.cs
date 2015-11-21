using NuClear.Metamodeling.Elements;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IStatisticsProcessorFactory
    {
        IStatisticsProcessor Create(IMetadataElement metadata);
    }
}