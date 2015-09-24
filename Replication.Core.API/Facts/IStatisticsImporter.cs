using System.Collections.Generic;

using NuClear.Replication.Metadata.Model;
using NuClear.Replication.Metadata.Operations;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IStatisticsImporter
    {
        IReadOnlyCollection<RecalculateStatisticsOperation> Import(IStatisticsDto statisticsDto);
    }
}