using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Bit;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public class BitTransformationContext : IBitContext
    {
        private readonly CategoryStatististicsDto _categoryStatististicsDto;
        private readonly FirmStatisticsDto _firmStatisticsDto;

        public BitTransformationContext(FirmStatisticsDto firmStatisticsDto)
        {
            _firmStatisticsDto = firmStatisticsDto;
            _categoryStatististicsDto = null;
        }

        public BitTransformationContext(CategoryStatististicsDto categoryStatististicsDto)
        {
            _categoryStatististicsDto = categoryStatististicsDto;
            _firmStatisticsDto = null;
        }

        public IQueryable<FirmStatistics> FirmStatistics
        {
            get
            {
                return _firmStatisticsDto == null
                           ? Enumerable.Empty<FirmStatistics>().AsQueryable()
                           : _firmStatisticsDto.Firms
                                               .Select(dto => new FirmStatistics
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

        public IQueryable<CategoryStatististics> CategoryStatististics
        {
            get
            {
                return _categoryStatististicsDto == null
                           ? Enumerable.Empty<CategoryStatististics>().AsQueryable()
                           : _categoryStatististicsDto.Categories
                                                      .Select(dto => new CategoryStatististics
                                                                     {
                                                                         ProjectId = _categoryStatististicsDto.ProjectId,
                                                                         CategoryId = dto.CategoryId,
                                                                         AdvertisersCount = dto.AdvertisersCount,
                                                                     })
                                                      .AsQueryable();
            }
        }
    }
}