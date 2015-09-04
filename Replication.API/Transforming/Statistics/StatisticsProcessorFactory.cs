using System;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public class StatisticsProcessorFactory : IStatisticsProcessorFactory
    {
        public IStatisticsProcessor Create(IStatisticsInfo metadata)
        {
            var type = typeof(StatisticsProcessor<>).MakeGenericType(metadata.Type);
            var processor = Activator.CreateInstance(type, metadata);
            return (IStatisticsProcessor)processor;
        }
    }
}