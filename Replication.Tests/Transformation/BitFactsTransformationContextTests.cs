using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    [TestFixture]
    internal class BitFactsTransformationContextTests
    {
        [Test]
        public void ShouldTransformFirmStatisticsDto()
        {
            var dto = new FirmStatisticsDto
                      {
                          ProjectId = 1,
                          Firms = new[]
                                  {
                                      new FirmStatisticsDto.FirmDto
                                      {
                                          FirmId = 2,
                                          CategoryId = 3,
                                          Hits = 4,
                                          Shows = 5
                                      }
                                  }
                      };
            var context = new BitFactsTransformationContext(dto);

            var entities = context.FirmStatistics;
            Assert.AreEqual(entities.Count(), 1);
            Assert.AreEqual(entities.Single().ProjectId, dto.ProjectId);
            Assert.AreEqual(entities.Single().CategoryId, dto.Firms.Single().CategoryId);
            Assert.AreEqual(entities.Single().FirmId, dto.Firms.Single().FirmId);
            Assert.AreEqual(entities.Single().Hits, dto.Firms.Single().Hits);
            Assert.AreEqual(entities.Single().Shows, dto.Firms.Single().Shows);
        }

        [Test]
        public void ShouldTransformCategoryStatisticsDto()
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
            var context = new BitFactsTransformationContext(dto);

            var entities = context.CategoryStatistics;

            Assert.AreEqual(entities.Count(), 1);
            Assert.AreEqual(entities.Single().ProjectId, dto.ProjectId);
            Assert.AreEqual(entities.Single().CategoryId, dto.Categories.Single().CategoryId);
            Assert.AreEqual(entities.Single().AdvertisersCount, dto.Categories.Single().AdvertisersCount);
        }
    }
}