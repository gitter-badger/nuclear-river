using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal static class DtoExtensions
    {
        public static IReadOnlyCollection<FirmCategoryStatistics> ToFirmCategoryStatistics(this FirmStatisticsDto dto)
        {
            return dto.Firms
                      .Select(x => new FirmCategoryStatistics
                                   {
                                       ProjectId = dto.ProjectId,
                                       FirmId = x.FirmId,
                                       CategoryId = x.CategoryId,
                                       Hits = x.Hits,
                                       Shows = x.Shows,
                                   })
                      .ToArray();
        }

        public static IReadOnlyCollection<ProjectCategoryStatistics> ToProjectCategoryStatistics(this CategoryStatisticsDto dto)
        {
            return dto.Categories
                      .Select(x => new ProjectCategoryStatistics
                                   {
                                       ProjectId = dto.ProjectId,
                                       CategoryId = x.CategoryId,
                                       AdvertisersCount = x.AdvertisersCount,
                                   })
                      .ToArray();
        }
    }
}