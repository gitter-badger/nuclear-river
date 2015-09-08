using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;

using NUnit.Framework;

using FirmCategoryStatistics = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.FirmCategoryStatistics;

namespace NuClear.AdvancedSearch.Replication.Tests.StatisticsTransformation
{
    [TestFixture]
    internal sealed class StatisticsFinalTransformationTests
    {
        [Test]
        public void ShouldRecalculateOnlySpecifiedProjectCategory()
        {
            Mock<IDataMapper> mapper;
            var transformation = CreateTransformation(out mapper);
            var operation = new CalculateStatisticsOperation { ProjectId = 1, CategoryId = 100 };

            transformation.Recalculate(new[] { operation });

            mapper.Verify(x => x.Insert(It.IsAny<FirmCategoryStatistics>()), Times.Never);
            mapper.Verify(x => x.Delete(It.IsAny<FirmCategoryStatistics>()), Times.Never);
            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.ProjectId != 1)), Times.Never);
            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.CategoryId != 100)), Times.Never);

            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 100)), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldRecalculateOnlySpecifiedProject()
        {
            Mock<IDataMapper> mapper;
            var transformation = CreateTransformation(out mapper);
            var operation = new CalculateStatisticsOperation { ProjectId = 1, CategoryId = null };

            transformation.Recalculate(new[] { operation });

            mapper.Verify(x => x.Insert(It.IsAny<FirmCategoryStatistics>()), Times.Never);
            mapper.Verify(x => x.Delete(It.IsAny<FirmCategoryStatistics>()), Times.Never);
            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.ProjectId != 1)), Times.Never);

            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 100)), Times.AtLeastOnce);
            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 101)), Times.AtLeastOnce);
            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 102)), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldUpdateOnlyChangedRecords()
        {
            Mock<IDataMapper> mapper;
            var transformation = CreateTransformationWithDataIntersection(out mapper);
            var operation = new CalculateStatisticsOperation { ProjectId = 1, CategoryId = null };

            transformation.Recalculate(new[] { operation });

            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.CategoryId == 100)), Times.Never);
            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.CategoryId == 101)), Times.AtLeastOnce);
        }

        private static StatisticsFinalTransformation CreateTransformation(out Mock<IDataMapper> mapperMock)
        {
            var data = new[]
                       {
                           new FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 100 },
                           new FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 101 },
                           new FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 102 },
                           new FirmCategoryStatistics { ProjectId = 1, FirmId = 11, CategoryId = 100 },
                           new FirmCategoryStatistics { ProjectId = 1, FirmId = 11, CategoryId = 101 },
                           new FirmCategoryStatistics { ProjectId = 2, FirmId = 12, CategoryId = 100 },
                       };

            return CreateTransformation(data,
                                        data.Select(x => new FirmCategoryStatistics { ProjectId = x.ProjectId, FirmId = x.FirmId, CategoryId = x.CategoryId, FirmCount = 1 }),
                                        out mapperMock);
        }

        private static StatisticsFinalTransformation CreateTransformationWithDataIntersection(out Mock<IDataMapper> mapperMock)
        {
            var sourceData = new[]
                             {
                                 new FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 100, Shows = 1 },
                                 new FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 101, Shows = 1 },
                             };

            var targetData = new[]
                             {
                                 new FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 100, Shows = 1 },
                                 new FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 101, Shows = 2 },
                             };

            return CreateTransformation(sourceData, targetData, out mapperMock);
        }

        private static StatisticsFinalTransformation CreateTransformation(IEnumerable<FirmCategoryStatistics> sourceData, IEnumerable<FirmCategoryStatistics> targetData, out Mock<IDataMapper> mapperMock)
        {
            var source = new Mock<IStatisticsContext>();
            source.SetupGet(x => x.FirmCategoryStatistics).Returns(sourceData.AsQueryable());
            var target = new Mock<IStatisticsContext>();
            target.SetupGet(x => x.FirmCategoryStatistics).Returns(targetData.AsQueryable());
            var mapper = new Mock<IDataMapper>();
            var transformation = new StatisticsFinalTransformation(source.Object, target.Object, mapper.Object);

            mapperMock = mapper;
            return transformation;
        }

        private static IQueryable<T> Inquire<T>(params T[] elements)
        {
            return elements.AsQueryable();
        }
    }
}
