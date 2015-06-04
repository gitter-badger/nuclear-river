using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context
{
    public interface IBitFactsContext
    {
        IQueryable<FirmStatistics> FirmStatistics { get; }

        IQueryable<CategoryStatistics> CategoryStatistics { get; }
    }
}