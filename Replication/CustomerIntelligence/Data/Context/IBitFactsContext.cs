using System.Linq;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context
{
    using Facts = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

    public interface IBitFactsContext
    {
        IQueryable<Facts.FirmCategory> FirmCategory { get; }
        IQueryable<Facts::FirmCategoryStatistics> FirmStatistics { get; }
        IQueryable<Facts::ProjectCategoryStatistics> CategoryStatistics { get; }
    }
}