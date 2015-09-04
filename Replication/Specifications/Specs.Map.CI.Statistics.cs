using System.Linq;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class CI
            {
                public static partial class ToStatistics
                {
                    public static readonly MapSpecification<IQuery, IQueryable<CI::FirmCategoryStatistics>> FirmCategoryStatistics =
                        new MapSpecification<IQuery, IQueryable<CI::FirmCategoryStatistics>>(
                            q => from firm in q.For<CI::Firm>()
                                 join statistics in q.For<CI::FirmCategoryStatistics>() on firm.Id equals statistics.FirmId
                                 select new CI::FirmCategoryStatistics
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