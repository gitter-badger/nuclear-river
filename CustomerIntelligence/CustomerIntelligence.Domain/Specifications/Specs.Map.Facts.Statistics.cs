using System;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Facts
            {
                public static partial class ToStatistics
                {
                    public static readonly MapSpecification<IQuery, IQueryable<Statistics::FirmCategoryStatistics>> FirmCategoryStatistics =
                        new MapSpecification<IQuery, IQueryable<Statistics::FirmCategoryStatistics>>(
                            q =>
                            {
                                var firmCounts = q.For<Bit::FirmCategory>()
                                                  .GroupBy(x => new { x.ProjectId, x.CategoryId })
                                                  .Select(x => new { x.Key.ProjectId, x.Key.CategoryId, Count = x.Count() });

                                return from firm in q.For<Bit::FirmCategory>()
                                       from firmStatistics in q.For<Bit::FirmCategoryStatistics>()
                                                               .Where(x => x.FirmId == firm.FirmId && x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId)
                                                               .DefaultIfEmpty(new Bit::FirmCategoryStatistics())
                                       from categoryStatistics in q.For<Bit::ProjectCategoryStatistics>()
                                                                   .Where(x => x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId)
                                                                   .DefaultIfEmpty(new Bit::ProjectCategoryStatistics())
                                       from firmCount in firmCounts.Where(x => x.CategoryId == firm.CategoryId && x.ProjectId == firm.ProjectId)
                                                                   .DefaultIfEmpty()
                                       select new Statistics::FirmCategoryStatistics
                                       {
                                           ProjectId = firm.ProjectId,
                                           FirmId = firm.FirmId,
                                           CategoryId = firm.CategoryId,
                                           Hits = firmStatistics != null ? firmStatistics.Hits : 0,
                                           Shows = firmStatistics != null ? firmStatistics.Shows : 0,
                                           FirmCount = firmCount.Count,
                                           AdvertisersShare = Math.Min(1, (float)categoryStatistics.AdvertisersCount / firmCount.Count)
                                       };
                            });
                }
            }
        }
    }
}