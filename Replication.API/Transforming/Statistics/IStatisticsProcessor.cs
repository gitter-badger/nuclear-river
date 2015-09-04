using System.Collections.Generic;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public interface IStatisticsProcessor
    {
        void RecalculateStatistics(IQuery query, IDataChangesApplier changesApplier, long projectId, IReadOnlyCollection<long?> categoryIds);
    }
}