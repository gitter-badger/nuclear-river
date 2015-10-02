using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IStatisticsImporter
    {
        IReadOnlyCollection<RecalculateStatisticsOperation> Import(IStatisticsDto statisticsDto);
    }
}