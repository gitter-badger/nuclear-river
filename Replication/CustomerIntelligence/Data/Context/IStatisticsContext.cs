using System.Linq;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

    public interface IStatisticsContext
    {
        IQueryable<CI.FirmCategoryStatistics> FirmCategoryStatistics { get; }
    }
}