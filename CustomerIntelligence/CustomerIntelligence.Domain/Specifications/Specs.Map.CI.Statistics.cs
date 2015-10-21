using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class CI
            {
                public static partial class ToStatistics
                {
                    public static readonly MapSpecification<IQuery, IQueryable<Statistics::FirmCategoryStatistics>> FirmCategoryStatistics =
                        new MapSpecification<IQuery, IQueryable<Statistics::FirmCategoryStatistics>>(
                            q => from firm in q.For<CI::Firm>()
                                 join statistics in q.For<Statistics::FirmCategoryStatistics>() on firm.Id equals statistics.FirmId
                                 select new Statistics::FirmCategoryStatistics
                                 {
                                            ProjectId = firm.ProjectId,

                                            FirmId = statistics.FirmId,
                                            CategoryId = statistics.CategoryId,
                                            AdvertisersShare = statistics.AdvertisersShare,
                                            FirmCount = statistics.FirmCount,
                                            Hits = statistics.Hits,
                                            Shows = statistics.Shows,
                                        });
                }
            }
        }
    }
}