using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Moq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Storage.Writings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.StatisticsTransformation
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
    using Facts = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal sealed class StatisticsFinalTransformationTests
    {
        [Test]
        public void ShouldRecalculateOnlySpecifiedProjectCategory()
        {
            VerificationContainer mapper;
            var transformation = CreateTransformation(out mapper);
            var operation = new CalculateStatisticsOperation { ProjectId = 1, CategoryId = 100 };

            transformation.Recalculate(new[] { operation });

            var mock = mapper.Verify<CI::FirmCategoryStatistics>();
            mock.Verify(x => x.Add(It.IsAny<CI::FirmCategoryStatistics>()), Times.Never);
            mock.Verify(x => x.Delete(It.IsAny<CI::FirmCategoryStatistics>()), Times.Never);
            mock.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId != 1)), Times.Never);
            mock.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.CategoryId != 100)), Times.Never);

            mock.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 100)), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldRecalculateOnlySpecifiedProject()
        {
            VerificationContainer mapper;
            var transformation = CreateTransformation(out mapper);
            var operation = new CalculateStatisticsOperation { ProjectId = 1, CategoryId = null };

            transformation.Recalculate(new[] { operation });

            var mock = mapper.Verify<CI::FirmCategoryStatistics>();
            mock.Verify(x => x.Add(It.IsAny<CI::FirmCategoryStatistics>()), Times.Never);
            mock.Verify(x => x.Delete(It.IsAny<CI::FirmCategoryStatistics>()), Times.Never);
            mock.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId != 1)), Times.Never);

            mock.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 100)), Times.AtLeastOnce);
            mock.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 101)), Times.AtLeastOnce);
            mock.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.ProjectId == 1 && y.CategoryId == 102)), Times.AtLeastOnce);
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

        private static StatisticsFinalTransformation CreateTransformation(out VerificationContainer container)
        {
            var query = new MemoryMockQuery(
                new Facts::FirmCategory { ProjectId = 1, FirmId = 10, CategoryId = 100, },
                new Facts::FirmCategory { ProjectId = 1, FirmId = 10, CategoryId = 101, },
                new Facts::FirmCategory { ProjectId = 1, FirmId = 10, CategoryId = 102, },
                new Facts::FirmCategory { ProjectId = 1, FirmId = 11, CategoryId = 100, },
                new Facts::FirmCategory { ProjectId = 1, FirmId = 11, CategoryId = 101, },
                new Facts::FirmCategory { ProjectId = 2, FirmId = 12, CategoryId = 100, },

                new Facts::FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 100, },
                new Facts::FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 101, },
                new Facts::FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 102, },
                new Facts::FirmCategoryStatistics { ProjectId = 1, FirmId = 11, CategoryId = 100, },
                new Facts::FirmCategoryStatistics { ProjectId = 1, FirmId = 11, CategoryId = 101, },
                new Facts::FirmCategoryStatistics { ProjectId = 2, FirmId = 12, CategoryId = 100, },
                
                new Facts::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 100, },
                new Facts::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 101, },
                new Facts::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 102, },
                new Facts::ProjectCategoryStatistics { ProjectId = 2, CategoryId = 100, },

                new CI::Firm { Id = 10, ProjectId = 1, },
                new CI::Firm { Id = 11, ProjectId = 1, },
                new CI::Firm { Id = 12, ProjectId = 2, },

                new CI::FirmCategoryStatistics { FirmId = 10, CategoryId = 100, },
                new CI::FirmCategoryStatistics { FirmId = 10, CategoryId = 101, },
                new CI::FirmCategoryStatistics { FirmId = 10, CategoryId = 102, },
                new CI::FirmCategoryStatistics { FirmId = 11, CategoryId = 100, },
                new CI::FirmCategoryStatistics { FirmId = 11, CategoryId = 101, },
                new CI::FirmCategoryStatistics { FirmId = 12, CategoryId = 100, });

            container = new VerificationContainer();
            return new StatisticsFinalTransformation(query, new VerifiableDataChangesApplierFactory(container.Add));
        }

        class VerificationContainer
                       {
            private readonly IDictionary<Type, IRepository> _dictionary = new Dictionary<Type, IRepository>();

            public Mock<IRepository<T>> Verify<T>() where T : class
            {
                var repository = (IRepository<T>)_dictionary[typeof(T)];
                return Mock.Get(repository);
        }

            public void Add(Type type, IRepository repository)
        {
                _dictionary.Add(type, repository);
            }
        }
    }
}
