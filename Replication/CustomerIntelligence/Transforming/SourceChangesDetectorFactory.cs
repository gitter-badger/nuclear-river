using System;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public class SourceChangesDetectorFactory : ISourceChangesDetectorFactory
    {
        public ISourceChangesDetector Create(ErmFactInfo factInfo, IQuery sourceQuery, IQuery destQuery)
        {
            var genericType = typeof(SourceChangesDetector<>).MakeGenericType(factInfo.FactType);
            return (ISourceChangesDetector)Activator.CreateInstance(genericType, factInfo, sourceQuery, destQuery);
        }
    }
}