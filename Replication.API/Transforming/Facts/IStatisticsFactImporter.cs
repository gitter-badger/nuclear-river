using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public interface IStatisticsFactImporter
    {
        IReadOnlyCollection<CalculateStatisticsOperation> Import(IStatisticsDto statisticsDto);
    }
}