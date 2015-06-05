using System.Linq;

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

            var entities = dto.ToFirmCategoryStatistics();

            Assert.That(entities.Count(), Is.EqualTo(1));
            Assert.That(entities.Single().ProjectId, Is.EqualTo(1));
            Assert.That(entities.Single().CategoryId, Is.EqualTo(3));
            Assert.That(entities.Single().FirmId, Is.EqualTo(2));
            Assert.That(entities.Single().Hits, Is.EqualTo(4));
            Assert.That(entities.Single().Shows, Is.EqualTo(5));
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

            var entities = dto.ToProjectCategoryStatistics();

            Assert.That(entities.Count(), Is.EqualTo(1));
            Assert.That(entities.Single().ProjectId, Is.EqualTo(1));
            Assert.That(entities.Single().CategoryId, Is.EqualTo(2));
            Assert.That(entities.Single().AdvertisersCount, Is.EqualTo(3));
        }
    }
}