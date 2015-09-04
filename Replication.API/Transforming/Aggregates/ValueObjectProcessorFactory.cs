using System;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates
{
    public class ValueObjectProcessorFactory : IValueObjectProcessorFactory
    {
        public IValueObjectProcessor Create(IValueObjectInfo metadata)
        {
            var type = typeof(ValueObjectProcessor<>).MakeGenericType(metadata.Type);
            var processor = Activator.CreateInstance(type, metadata);
            return (IValueObjectProcessor)processor;
        }
    }
}