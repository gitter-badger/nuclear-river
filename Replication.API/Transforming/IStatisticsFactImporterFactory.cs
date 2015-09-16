using System;

using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface IStatisticsFactImporterFactory
    {
        IStatisticsFactImporter Create(Type statisticsDtoType);
    }
}