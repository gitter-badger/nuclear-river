using System;
using System.Collections.Generic;

using Moq;

using NuClear.AdvancedSearch.Replication.API.Operations;
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
        private static object[] data =
            new object[]
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
            VerificationContainer mapper;
            var transformation = CreateTransformation(data, out mapper);
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
            var transformation = CreateTransformation(data, out mapper);
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
            VerificationContainer mapper;
            var transformation = CreateTransformation(data, out mapper);
            var operation = new CalculateStatisticsOperation { ProjectId = 3, CategoryId = null };

            transformation.Recalculate(new[] { operation });

            var mock = mapper.Verify<CI::FirmCategoryStatistics>();
            mock.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.CategoryId == 100)), Times.Never);
            mock.Verify(x => x.Update(It.Is<CI::FirmCategoryStatistics>(y => y.CategoryId == 101)), Times.Once);
        }

        private static StatisticsFinalTransformation CreateTransformation(object[] data, out VerificationContainer container)
        {
            var query = new MemoryMockQuery(data);
            container = new VerificationContainer();
            return new StatisticsFinalTransformation(query, new VerifiableDataChangesApplierFactory(container.Add));
        }

        private class VerificationContainer
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
