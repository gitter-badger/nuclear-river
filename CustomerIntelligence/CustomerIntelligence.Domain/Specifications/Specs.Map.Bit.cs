using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Bit
            {
                public static MapSpecification<IStatisticsDto, IReadOnlyCollection<Bit::FirmCategoryStatistics>> FirmCategoryStatistics()
                {
                    return new MapSpecification<IStatisticsDto, IReadOnlyCollection<Bit::FirmCategoryStatistics>>(
                        dto =>
                        {
                            var statisticsDto = (FirmStatisticsDto)dto;
                            return statisticsDto.Firms
                                                .SelectMany(x => x.Categories.Select(y => new Bit::FirmCategoryStatistics
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

                public static MapSpecification<IStatisticsDto, IReadOnlyCollection<Bit::ProjectCategoryStatistics>> ProjectCategoryStatistics()
                {
                    return new MapSpecification<IStatisticsDto, IReadOnlyCollection<Bit::ProjectCategoryStatistics>>(
                        dto =>
                        {
                            var statisticsDto = (CategoryStatisticsDto)dto;
                            return statisticsDto.Categories
                                                .Select(x => new Bit::ProjectCategoryStatistics
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