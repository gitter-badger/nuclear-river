using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IStatisticsProcessor
    {
        void RecalculateStatistics(long projectId, IReadOnlyCollection<long?> categoryIds);
    }
}