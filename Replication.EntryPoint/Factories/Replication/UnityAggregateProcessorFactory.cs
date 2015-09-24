using System;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API.Aggregates;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication
{
    public sealed class UnityAggregateProcessorFactory : IAggregateProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityAggregateProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IAggregateProcessor Create(Type aggregateType, IMetadataElement aggregateMetadata)
        {
            var processorType = typeof(AggregateProcessor<>).MakeGenericType(aggregateType);
            var processor = _unityContainer.Resolve(processorType, new DependencyOverride(aggregateMetadata.GetType(), aggregateMetadata));
            return (IAggregateProcessor)processor;
        }
    }
}