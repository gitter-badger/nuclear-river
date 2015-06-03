using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Bit;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context
{
    public interface IBitContext
    {
        IQueryable<FirmStatistics> FirmStatistics { get; }

        IQueryable<CategoryStatististics> CategoryStatististics { get; }
    }
}