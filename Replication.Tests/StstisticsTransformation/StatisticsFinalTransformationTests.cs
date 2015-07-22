using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;

using NUnit.Framework;

using FirmCategoryStatistics = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.FirmCategoryStatistics;

namespace NuClear.AdvancedSearch.Replication.Tests.StstisticsTransformation
{
    [TestFixture]
    internal sealed class StatisticsFinalTransformationTests
    {
        [Test]
        public void ShouldRecalculateOnlySpecifiedProjectCategory()
        {
            Mock<IDataMapper> mapper;
            var transformation = CreateTransformation(out mapper);
            var operation = new StatisticsOperation { ProjectId = 1, CategoryId = 100 };

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
            var operation = new StatisticsOperation { ProjectId = 1, CategoryId = null };

            transformation.Recalculate(new[] { operation });

            mapper.Verify(x => x.Insert(It.IsAny<FirmCategoryStatistics>()), Times.Never);
            mapper.Verify(x => x.Delete(It.IsAny<FirmCategoryStatistics>()), Times.Never);
            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.ProjectId != 1)), Times.Never);

            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 100)), Times.AtLeastOnce);
            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 101)), Times.AtLeastOnce);
            mapper.Verify(x => x.Update(It.Is<FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 102)), Times.AtLeastOnce);
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

            var source = new Mock<IStatisticsContext>();
            source.SetupGet(x => x.FirmCategoryStatistics).Returns(Inquire(data));
            var target = new Mock<IStatisticsContext>();
            target.SetupGet(x => x.FirmCategoryStatistics).Returns(Inquire(data));
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
