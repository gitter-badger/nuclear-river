using System;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    internal class FactDependencyProcessorFactory : IFactDependencyProcessorFactory
    {
        public IFactDependencyProcessor Create(IFactDependencyInfo metadata)
        {
            var type = typeof(FactDependencyProcessor<>).MakeGenericType(metadata.Type);
            var processor = Activator.CreateInstance(type, metadata);
            return (IFactDependencyProcessor)processor;
        }
    }
}