using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.CustomerIntelligence.Domain.Model.Facts;
using NuClear.Replication.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Bit
            {
                public static MapSpecification<IStatisticsDto, IReadOnlyCollection<FirmCategoryStatistics>> FirmCategoryStatistics()
                {
                    return new MapSpecification<IStatisticsDto, IReadOnlyCollection<FirmCategoryStatistics>>(
                        dto =>
                        {
                            var statisticsDto = (FirmStatisticsDto)dto;
                            return statisticsDto.Firms
                                                .SelectMany(x => x.Categories.Select(y => new FirmCategoryStatistics
                                                                                          {
                                                                                              ProjectId = statisticsDto.ProjectId,
                                                                                              FirmId = x.FirmId,
                                                                                              CategoryId = y.CategoryId,
                                                                                              Hits = y.Hits,
                                                                                              Shows = y.Shows,
                                                                                          }))
                                                .ToArray();
                        });
                }

                public static MapSpecification<IStatisticsDto, IReadOnlyCollection<ProjectCategoryStatistics>> ProjectCategoryStatistics()
                {
                    return new MapSpecification<IStatisticsDto, IReadOnlyCollection<ProjectCategoryStatistics>>(
                        dto =>
                        {
                            var statisticsDto = (CategoryStatisticsDto)dto;
                            return statisticsDto.Categories
                                                .Select(x => new ProjectCategoryStatistics
                                                             {
                                                                 ProjectId = statisticsDto.ProjectId,
                                                                 CategoryId = x.CategoryId,
                                                                 AdvertisersCount = x.AdvertisersCount
                                                             })
                                                .ToArray();
                        });
                }
            }
        }
    }
}