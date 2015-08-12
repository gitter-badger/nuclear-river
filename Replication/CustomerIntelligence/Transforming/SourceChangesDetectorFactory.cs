using System;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public class SourceChangesDetectorFactory : ISourceChangesDetectorFactory
    {
        public ISourceChangesDetector Create(IFactInfo factInfo, IQuery source, IQuery target)
        {
            var genericType = typeof(SourceChangesDetector<>).MakeGenericType(factInfo.Type);
            return (ISourceChangesDetector)Activator.CreateInstance(genericType, factInfo, source, target);
        }
    }
}