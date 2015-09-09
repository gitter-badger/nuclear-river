using Moq;

using NuClear.AdvancedSearch.Replication.API.Transforming.Statistics;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Writings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.StatisticsTransformation
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
    using Facts = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal sealed class StatisticsFinalTransformationTests
    {
        private static readonly object[] data =
            {
                new Facts::FirmCategory { ProjectId = 1, FirmId = 10, CategoryId = 100, },
                new Facts::FirmCategory { ProjectId = 1, FirmId = 10, CategoryId = 101, },
                new Facts::FirmCategory { ProjectId = 1, FirmId = 10, CategoryId = 102, },
                new Facts::FirmCategory { ProjectId = 1, FirmId = 11, CategoryId = 100, },
                new Facts::FirmCategory { ProjectId = 1, FirmId = 11, CategoryId = 101, },
                new Facts::FirmCategory { ProjectId = 2, FirmId = 12, CategoryId = 100, },
                new Facts::FirmCategory { ProjectId = 3, FirmId = 10, CategoryId = 100, },
                new Facts::FirmCategory { ProjectId = 3, FirmId = 10, CategoryId = 101, },

                new Facts::FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 100, },
                new Facts::FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 101, },
                new Facts::FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 102, },
                new Facts::FirmCategoryStatistics { ProjectId = 1, FirmId = 11, CategoryId = 100, },
                new Facts::FirmCategoryStatistics { ProjectId = 1, FirmId = 11, CategoryId = 101, },
                new Facts::FirmCategoryStatistics { ProjectId = 2, FirmId = 12, CategoryId = 100, },
                new Facts::FirmCategoryStatistics { ProjectId = 3, FirmId = 10, CategoryId = 100, Hits = 1, Shows = 2 },
                new Facts::FirmCategoryStatistics { ProjectId = 3, FirmId = 10, CategoryId = 101, Hits = 1, Shows = 2 },

                new Facts::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 100, },
                new Facts::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 101, },
                new Facts::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 102, },
                new Facts::ProjectCategoryStatistics { ProjectId = 2, CategoryId = 100, },
                new Facts::ProjectCategoryStatistics { ProjectId = 3, CategoryId = 100, AdvertisersCount = 2 },
                new Facts::ProjectCategoryStatistics { ProjectId = 3, CategoryId = 101, AdvertisersCount = 2 },

                new CI::Firm { Id = 10, ProjectId = 1, },
                new CI::Firm { Id = 11, ProjectId = 1, },
                new CI::Firm { Id = 12, ProjectId = 2, },
                new CI::Firm { Id = 10, ProjectId = 3, },

                new CI::FirmCategoryStatistics { FirmId = 10, CategoryId = 100, },
                new CI::FirmCategoryStatistics { FirmId = 10, CategoryId = 101, },
                new CI::FirmCategoryStatistics { FirmId = 10, CategoryId = 102, },
                new CI::FirmCategoryStatistics { FirmId = 11, CategoryId = 100, },
                new CI::FirmCategoryStatistics { FirmId = 11, CategoryId = 101, },
                new CI::FirmCategoryStatistics { FirmId = 12, CategoryId = 100, },
                new CI::FirmCategoryStatistics { FirmId = 10, CategoryId = 100, Shows = 2, Hits = 1, AdvertisersShare = 2f / 1, FirmCount = 1 },
                new CI::FirmCategoryStatistics { FirmId = 10, CategoryId = 101, },
            };

        [Test]
        public void ShouldRecalculateOnlySpecifiedProjectCategory()
        {
            Mock<IRepository<CI::FirmCategoryStatistics>> repository;
            var processor = StatisticsProcessor(data, out repository);

            processor.RecalculateStatistics(1, new long?[] { 100 });

            repository.Verify(x => x.Add(It.IsAny<CI::FirmCategoryStatistics>()), Times.Never);
            repository.Verify(x => x.Delete(It.IsAny<CI::FirmCategoryStatistics>()), Times.Never);
            repository.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId != 1)), Times.Never);
            repository.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.CategoryId != 100)), Times.Never);

            repository.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 100)), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldRecalculateOnlySpecifiedProject()
        {
            Mock<IRepository<CI::FirmCategoryStatistics>> repository;
            var processor = StatisticsProcessor(data, out repository);

            processor.RecalculateStatistics(1, new long?[] { null });

            repository.Verify(x => x.Add(It.IsAny<CI::FirmCategoryStatistics>()), Times.Never);
            repository.Verify(x => x.Delete(It.IsAny<CI::FirmCategoryStatistics>()), Times.Never);
            repository.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId != 1)), Times.Never);

            repository.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 100)), Times.AtLeastOnce);
            repository.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 101)), Times.AtLeastOnce);
            repository.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 102)), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldUpdateOnlyChangedRecords()
        {
            Mock<IRepository<CI::FirmCategoryStatistics>> repository;
            var processor = StatisticsProcessor(data, out repository);

            processor.RecalculateStatistics(3, new long?[] { null });

            repository.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.CategoryId == 100)), Times.Never);
            repository.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.CategoryId == 101)), Times.Once);
        }

        private static IStatisticsProcessor StatisticsProcessor<T>(object[] data, out Mock<IRepository<T>> repository) 
            where T : class
        {
            var metadataSource = new StatisticsFinalTransformationMetadata();
            var metadata = metadataSource.Metadata[typeof(T)];
            repository = new Mock<IRepository<T>>();

            return new StatisticsProcessor<T>(
                (StatisticsInfo<T>)metadata,
                new MemoryMockQuery(data),
                new DataChangesApplier<T>(repository.Object));
        }
    }
}
