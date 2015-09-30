using System.Linq;

using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class CI
            {
                public static partial class ToStatistics
                {
                    public static readonly MapSpecification<IQuery, IQueryable<FirmCategoryStatistics>> FirmCategoryStatistics =
                        new MapSpecification<IQuery, IQueryable<FirmCategoryStatistics>>(
                            q => from firm in q.For<Firm>()
                                 join statistics in q.For<FirmCategoryStatistics>() on firm.Id equals statistics.FirmId
                                 select new FirmCategoryStatistics
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