using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;
    using CI = CustomerIntelligence.Model;

    [TestFixture]
    internal class BitFactsTransformationTests
    {
        [Test]
        public void ShouldProduceRecalulateOperationsBothForExistedAndNewFirmStatistics()
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
            var context = new Mock<IBitFactsContext>();
            context.SetupGet(x => x.FirmStatistics).Returns(new [] {new Facts.FirmStatistics { ProjectId = 1, FirmId = 7}, new Facts.FirmStatistics { ProjectId = 2, FirmId = 8 } }.AsQueryable());
            var transformation = new BitFactsTransformation(context.Object, Mock.Of<IDataMapper>());

            var operations = transformation.Transform(dto).ToArray();

            Assert.AreEqual(2, operations.Count());
            Assert.AreEqual(2, operations.OfType<RecalculateAggregate>().Count());
            Assert.AreEqual(2, operations.OfType<RecalculateAggregate>().Count(x => x.AggregateType == typeof(CI.Firm)));
            Assert.AreEqual(1, operations.OfType<RecalculateAggregate>().Count(x => x.AggregateId == 2));
            Assert.AreEqual(1, operations.OfType<RecalculateAggregate>().Count(x => x.AggregateId == 7));
        }

        [Test]
        public void ShouldProduceRecalulateOperationsForReceivedProject()
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
            var context = new Mock<IBitFactsContext>();
            context.SetupGet(x => x.CategoryStatististics).Returns(new[] { new Facts.CategoryStatististics { ProjectId = 1, CategoryId = 7 }, new Facts.CategoryStatististics { ProjectId = 2, CategoryId = 7 } }.AsQueryable());
            var transformation = new BitFactsTransformation(context.Object, Mock.Of<IDataMapper>());

            var operations = transformation.Transform(dto).ToArray();

            Assert.AreEqual(1, operations.Count());
            Assert.AreEqual(1, operations.OfType<RecalculateAggregate>().Count());
            Assert.AreEqual(1, operations.OfType<RecalculateAggregate>().Count(x => x.AggregateType == typeof(CI.Project)));
            Assert.AreEqual(1, operations.OfType<RecalculateAggregate>().Count(x => x.AggregateId == 1));
        }
    }
}