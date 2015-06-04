using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public class BitFactsTransformationContext : IBitFactsContext
    {
        private readonly CategoryStatisticsDto _categoryStatisticsDto;
        private readonly FirmStatisticsDto _firmStatisticsDto;

        public BitFactsTransformationContext(FirmStatisticsDto firmStatisticsDto)
        {
            _firmStatisticsDto = firmStatisticsDto;
            _categoryStatisticsDto = null;
        }

        public BitFactsTransformationContext(CategoryStatisticsDto categoryStatisticsDto)
        {
            _categoryStatisticsDto = categoryStatisticsDto;
            _firmStatisticsDto = null;
        }

        public IQueryable<FirmCategoryStatistics> FirmStatistics
        {
            get
            {
                return _firmStatisticsDto == null
                           ? Enumerable.Empty<FirmCategoryStatistics>().AsQueryable()
                           : _firmStatisticsDto.Firms
                                               .Select(dto => new FirmCategoryStatistics
                                               {
                                                                  ProjectId = _firmStatisticsDto.ProjectId,
                                                                  FirmId = dto.FirmId,
                                                                  CategoryId = dto.CategoryId,
                                                                  Hits = dto.Hits,
                                                                  Shows = dto.Shows,
                                                              })
                                               .AsQueryable();
            }
        }

        public IQueryable<ProjectCategoryStatistics> CategoryStatistics
        {
            get
            {
                return _categoryStatisticsDto == null
                           ? Enumerable.Empty<ProjectCategoryStatistics>().AsQueryable()
                           : _categoryStatisticsDto.Categories
                                                      .Select(dto => new ProjectCategoryStatistics
                                                      {
                                                                         ProjectId = _categoryStatisticsDto.ProjectId,
                                                                         CategoryId = dto.CategoryId,
                                                                         AdvertisersCount = dto.AdvertisersCount,
                                                                     })
                                                      .AsQueryable();
            }
        }
    }
}