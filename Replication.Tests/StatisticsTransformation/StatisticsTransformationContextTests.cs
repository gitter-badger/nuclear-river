using System;
using System.Collections.Generic;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.Tests.Data;
using NuClear.AdvancedSearch.Replication.Tests.Transformation;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.StatisticsTransformation
{
    using CI = CustomerIntelligence.Model;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture, SetCulture("")]
    internal class StatisticsTransformationContextTests : BaseTransformationFixture
    {
        [Test]
        public void ShouldFillCategoriesWithoutStatisticsWithZeros()
        {
            FactsDb.Has(new Facts::FirmCategory { FirmId = 1, CategoryId = 1, ProjectId = 1 }); // Фирма без статистики
            FactsDb.Has(new Facts::FirmCategory { FirmId = 2, CategoryId = 1, ProjectId = 1 }); // Фирма со статистикой
            FactsDb.Has(new Facts::FirmCategoryStatistics { FirmId = 2, CategoryId = 1, ProjectId = 1, Hits = 100, Shows = 200 });
            FactsDb.Has(new Facts::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 1 });

            Transformation.Create(new BitFactsContext(FactsDb))
                          .VerifyTransform(x => x.FirmCategoryStatistics, Inquire(
                              new CI::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 0, Shows = 0 },
                              new CI::FirmCategoryStatistics { FirmId = 2, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 100, Shows = 200 }));
        }

        [Test]
        public void ShouldTransformFirmCategoryStatistics()
        {
            var context = new Mock<IBitFactsContext>();
            context.SetupGet(x => x.FirmCategory)
                   .Returns(Inquire(new Facts::FirmCategory { FirmId = 1, CategoryId = 1, ProjectId = 1 },
                                    new Facts::FirmCategory { FirmId = 2, CategoryId = 1, ProjectId = 1 },
                                    new Facts::FirmCategory { FirmId = 2, CategoryId = 2, ProjectId = 1 }));

            context.SetupGet(x => x.FirmStatistics)
                   .Returns(Inquire(new Facts::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, Hits = 10000, Shows = 20000 },
                                    new Facts::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 2, Hits = 10001, Shows = 20002 }));

            context.SetupGet(x => x.CategoryStatistics)
                   .Returns(Inquire(new Facts::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 1 }));

            Transformation.Create(context.Object)
                          .VerifyTransform(x => x.FirmCategoryStatistics, Inquire(
                              new CI::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 10000, Shows = 20000 },
                              new CI::FirmCategoryStatistics { FirmId = 2, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 0, Shows = 0 },
                              new CI::FirmCategoryStatistics { FirmId = 2, CategoryId = 2, ProjectId = 1, AdvertisersShare = 0f, FirmCount = 1, Hits = 0, Shows = 0 }));
        }

        [Test]
        public void AdvertisersShareShouldNotBeMoreThanOne()
        {
            var context = new Mock<IBitFactsContext>();
            context.SetupGet(x => x.FirmCategory)
                   .Returns(Inquire(new Facts::FirmCategory { FirmId = 1, CategoryId = 1, ProjectId = 1 }));

            context.SetupGet(x => x.FirmStatistics)
                   .Returns(Inquire(new Facts::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, Hits = 10000, Shows = 20000 }));

            context.SetupGet(x => x.CategoryStatistics)
                   .Returns(Inquire(new Facts::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 5 }));

            Transformation.Create(context.Object)
                          .VerifyTransform(x => x.FirmCategoryStatistics, Inquire(
                              new CI::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, AdvertisersShare = 1f, FirmCount = 1, Hits = 10000, Shows = 20000 }));
        }

        #region Transformation

        private class Transformation
        {
            private readonly IStatisticsContext _transformation;

            private Transformation(IBitFactsContext bitContext)
            {
                _transformation = new StatisticsTransformationContext(bitContext);
            }

            public static Transformation Create(IBitFactsContext bitContext)
            {
                return new Transformation(bitContext);
            }

            public Transformation VerifyTransform<T>(Func<IStatisticsContext, IEnumerable<T>> reader, IEnumerable<T> expected, string message = null)
            {
                VerifyTransform(reader, expected, x => x, message);
                return this;
            }

            public Transformation VerifyTransform<T, TProjection>(Func<IStatisticsContext, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                // TODO: convert to a custom NUnit constraint, at least for fail logging
                Assert.That(reader(_transformation), Is.EquivalentTo(expected).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }

        #endregion
    }
}