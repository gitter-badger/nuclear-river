using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    [TestFixture, SetCulture("")]
    internal class StatisticsTransformationContextTests : TransformationFixtureBase
    {
        [Test]
        public void ShouldFillCategoriesWithoutStatisticsWithZeros()
        {
            SourceDb.Has(new Bit::FirmCategory { FirmId = 1, CategoryId = 1, ProjectId = 1 }); // Фирма без статистики
            SourceDb.Has(new Bit::FirmCategory { FirmId = 2, CategoryId = 1, ProjectId = 1 }); // Фирма со статистикой
            SourceDb.Has(new Bit::FirmCategoryStatistics { FirmId = 2, CategoryId = 1, ProjectId = 1, Hits = 100, Shows = 200 });
            SourceDb.Has(new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 1 });

            Transformation.Create(Query)
                          .VerifyTransform(
                              x => Specs.Map.Facts.ToStatistics.FirmCategoryStatistics.Map(x),
                              Inquire(
                                  new Statistics::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 0, Shows = 0 },
                                  new Statistics::FirmCategoryStatistics { FirmId = 2, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 100, Shows = 200 }));
        }

        [Test]
        public void ShouldTransformFirmCategoryStatistics()
        {
            SourceDb.Has(new Bit::FirmCategory { FirmId = 1, CategoryId = 1, ProjectId = 1 },
                         new Bit::FirmCategory { FirmId = 2, CategoryId = 1, ProjectId = 1 },
                         new Bit::FirmCategory { FirmId = 2, CategoryId = 2, ProjectId = 1 })
                    .Has(new Bit::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, Hits = 10000, Shows = 20000 })
                    .Has(new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 1 });

            Transformation.Create(Query)
                          .VerifyTransform(
                              x => Specs.Map.Facts.ToStatistics.FirmCategoryStatistics.Map(x),
                              Inquire(
                                  new Statistics::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 10000, Shows = 20000 },
                                  new Statistics::FirmCategoryStatistics { FirmId = 2, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 0, Shows = 0 },
                                  new Statistics::FirmCategoryStatistics { FirmId = 2, CategoryId = 2, ProjectId = 1, AdvertisersShare = 0f, FirmCount = 1, Hits = 0, Shows = 0 }));
        }

        [Test]
        public void AdvertisersShareShouldNotBeMoreThanOne()
        {
            SourceDb.Has(new Bit::FirmCategory { FirmId = 1, CategoryId = 1, ProjectId = 1 })
                    .Has(new Bit::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, Hits = 10000, Shows = 20000 })
                    .Has(new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 5 });

            Transformation.Create(Query)
                          .VerifyTransform(
                              x => Specs.Map.Facts.ToStatistics.FirmCategoryStatistics.Map(x),
                              Inquire(new Statistics::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, AdvertisersShare = 1f, FirmCount = 1, Hits = 10000, Shows = 20000 }));
        }

        private class Transformation
        {
            private readonly IQuery _query;

            private Transformation(IQuery query)
            {
                _query = query;
            }

            public static Transformation Create(IQuery query)
            {
                return new Transformation(query);
            }

            public Transformation VerifyTransform<T>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, string message = null)
            {
                VerifyTransform(reader, expected, x => x, message);
                return this;
            }

            public Transformation VerifyTransform<T, TProjection>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                // TODO: convert to a custom NUnit constraint, at least for fail logging
                Assert.That(reader(_query), Is.EquivalentTo(expected).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }
    }
}