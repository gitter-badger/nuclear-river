using System;
using System.Linq;

using Moq;

using NuClear.CustomerIntelligence.Domain;
using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.Facts;
using NuClear.Replication.Metadata;
using NuClear.Replication.Metadata.Facts;

using NUnit.Framework;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    [TestFixture]
    internal class StatisticsImporterTests
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

            var metadataSource = new ImportStatisticsMetadataSource();
            IMetadataElement importStatisticsMetadata;
            Uri factMetadataId = Metadata.Id.For(ImportStatisticsMetadataIdentity.Instance.Id, typeof(FirmStatisticsDto).Name);
            if (!metadataSource.Metadata.TryGetValue(factMetadataId, out importStatisticsMetadata))
            {
                throw new NotSupportedException(string.Format("Import for statistics of type '{0}' is not supported.", typeof(FirmStatisticsDto)));
            }

            var importer = new StatisticsFactImporter<Facts.FirmCategoryStatistics>(
                (ImportStatisticsMetadata<Facts.FirmCategoryStatistics>)importStatisticsMetadata,
                query,
                Mock.Of<IBulkRepository<Facts.FirmCategoryStatistics>>());

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

            var metadataSource = new ImportStatisticsMetadataSource();
            IMetadataElement importStatisticsMetadata;
            Uri factMetadataId = Metadata.Id.For(ImportStatisticsMetadataIdentity.Instance.Id, typeof(CategoryStatisticsDto).Name);
            if (!metadataSource.Metadata.TryGetValue(factMetadataId, out importStatisticsMetadata))
            {
                throw new NotSupportedException(string.Format("Import for statistics of type '{0}' is not supported.", typeof(CategoryStatisticsDto)));
            }

            var importer = new StatisticsFactImporter<Facts.ProjectCategoryStatistics>(
                (ImportStatisticsMetadata<Facts.ProjectCategoryStatistics>)importStatisticsMetadata,
                query,
                Mock.Of<IBulkRepository<Facts.ProjectCategoryStatistics>>());

            var operations = importer.Import(dto).ToArray();

            Assert.That(operations.Count(), Is.EqualTo(1));
            Assert.That(operations.Count(), Is.EqualTo(1));
        }
    }
}