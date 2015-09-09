using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Statistics
{
    public interface IStatisticsProcessor
    {
        void RecalculateStatistics(long projectId, IReadOnlyCollection<long?> categoryIds);
    }
}