using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context
{
    public interface IBitFactsContext
    {
        IQueryable<FirmCategoryStatistics> FirmStatistics { get; }

        IQueryable<ProjectCategoryStatistics> CategoryStatistics { get; }
    }
}