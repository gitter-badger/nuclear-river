using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal static class BitDtoExtensions
    {
        public static IReadOnlyCollection<FirmCategoryStatistics> ToFirmCategoryStatistics(this FirmStatisticsDto dto)
        {
            return dto.Firms
                      .SelectMany(x => x.Categories.Select(y => new FirmCategoryStatistics
                                   {
                                       ProjectId = dto.ProjectId,
                                       FirmId = x.FirmId,
                                       CategoryId = y.CategoryId,
                                       Hits = y.Hits,
                                       Shows = y.Shows,
                                   }))
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