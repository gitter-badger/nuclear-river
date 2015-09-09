using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactDependencyProcessor
    {
        IEnumerable<IOperation> ProcessCreation(IReadOnlyCollection<long> factIds);
        IEnumerable<IOperation> ProcessUpdating(IReadOnlyCollection<long> factIds);
        IEnumerable<IOperation> ProcessDeletion(IReadOnlyCollection<long> factIds);
    }
}