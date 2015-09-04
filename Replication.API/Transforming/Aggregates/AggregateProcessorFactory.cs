using System;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public sealed class AggregateProcessorFactory : IAggregateProcessorFactory
    {
        public IAggregateProcessor Create(IAggregateInfo metadata)
        {
            var type = typeof(AggregateProcessor<>).MakeGenericType(metadata.Type);
            var processor = Activator.CreateInstance(type, metadata);
            return (IAggregateProcessor)processor;
        }
    }
}