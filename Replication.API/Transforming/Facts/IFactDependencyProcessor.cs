using System.Collections.Generic;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IFactDependencyProcessor
    {
        IEnumerable<IOperation> ProcessCreation(IQuery query, IReadOnlyCollection<long> factIds);
        IEnumerable<IOperation> ProcessUpdating(IQuery query, IReadOnlyCollection<long> factIds);
        IEnumerable<IOperation> ProcessDeletion(IQuery query, IReadOnlyCollection<long> factIds);
    }
}