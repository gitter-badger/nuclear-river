using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface ISourceChangesDetector
    {
        IMergeResult<long> DetectChanges(IReadOnlyCollection<long> ids); 
    }

    public interface ISourceChangesDetector<T> : ISourceChangesDetector where T : class, IIdentifiable
    {
    }
}