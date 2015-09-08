using System;

using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class VerifiableFactProcessorFactory : IFactProcessorFactory, IFactDependencyProcessorFactory
    {
        public IFactProcessor Create(IFactInfo factInfo)
        {
            var applierType = typeof(FactProcessor<>).MakeGenericType(factInfo.Type);
            return (IFactProcessor)Activator.CreateInstance(applierType, factInfo, this);
        }

        public IFactDependencyProcessor Create(IFactDependencyInfo metadata)
        {
            var applierType = typeof(FactDependencyProcessor<>).MakeGenericType(metadata.Type);
            return (IFactDependencyProcessor)Activator.CreateInstance(applierType, metadata);
        }
    }
}