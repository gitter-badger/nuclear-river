using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal class StatisticsFactImporterTests
    {
        [Test]
        public void ShouldProduceCalculateStatisticsOperationForFirmStatisticsDto()
        {
            var dto = new FirmStatisticsDto
                      {
                          ProjectId = 1,
                          Firms = new[]
                                  {
                                      new FirmStatisticsDto.FirmDto
                                      {
                                            FirmId = 2,
                                            Categories = new[]
                                            {
                                                new FirmStatisticsDto.FirmDto.CategoryDto
                                                {
                                                    CategoryId = 3,
                                                    Hits = 4,
                                                    Shows = 5
                                                }
                                            }
                                      }
                                  }
                      };

            var query = new MemoryMockQuery(
                new Facts.FirmCategoryStatistics { ProjectId = 1, FirmId = 7 },
                new Facts.FirmCategoryStatistics { ProjectId = 2, FirmId = 8 });

            var metadata = new StatisticsFactImportMetadata();
            var statisticsFactInfo = (IStatisticsFactInfo<Facts.FirmCategoryStatistics>)metadata.Metadata[typeof(FirmStatisticsDto)];
            var importer = new StatisticsFactImporter<Facts.FirmCategoryStatistics>(statisticsFactInfo, query, Mock.Of<IBulkRepository<Facts.FirmCategoryStatistics>>());

            var operations = importer.Import(dto).ToArray();

            Assert.That(operations.Count(), Is.EqualTo(1));
            Assert.That(operations.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ShouldProduceCalculateStatisticsOperationForCategoryStatisticsDto()
        {
            var dto = new CategoryStatisticsDto
            {
                ProjectId = 1,
                Categories = new[]
                                  {
                                      new CategoryStatisticsDto.CategoryDto
                                      {
                                          CategoryId = 2,
                                          AdvertisersCount = 3,
                                      }
                                  }
            };
            var query = new MemoryMockQuery(
                new Facts.ProjectCategoryStatistics { ProjectId = 1, CategoryId = 7 },
                new Facts.ProjectCategoryStatistics { ProjectId = 2, CategoryId = 7 });

            var metadata = new StatisticsFactImportMetadata();
            var statisticsFactInfo = (IStatisticsFactInfo<Facts.ProjectCategoryStatistics>)metadata.Metadata[typeof(CategoryStatisticsDto)];
            var importer = new StatisticsFactImporter<Facts.ProjectCategoryStatistics>(statisticsFactInfo, query, Mock.Of<IBulkRepository<Facts.ProjectCategoryStatistics>>());


            var operations = importer.Import(dto).ToArray();

            Assert.That(operations.Count(), Is.EqualTo(1));
            Assert.That(operations.Count(), Is.EqualTo(1));
        }
    }
}