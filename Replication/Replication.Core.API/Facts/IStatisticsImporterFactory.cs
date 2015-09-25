using System;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IStatisticsImporterFactory
    {
        IStatisticsImporter Create(Type statisticsDtoType);
    }
}