using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface ISourceChangesDetector
    {
        IMergeResult<long> DetectChanges(IReadOnlyCollection<long> factIds); 
    }

    public interface ISourceChangesDetector<TFact> : ISourceChangesDetector where TFact : class, IErmFactObject
    {
    }
}