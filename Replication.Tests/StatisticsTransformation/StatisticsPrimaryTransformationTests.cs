using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.StatisticsTransformation
{
    [TestFixture]
    internal sealed class StatisticsPrimaryTransformationTests
    {
        public IEnumerable<TestCaseData> SpecificCategoryCases()
        {
            yield return new TestCaseData(typeof(Firm), 1);
            yield return new TestCaseData(typeof(FirmAddress), 2);
            yield return new TestCaseData(typeof(CategoryFirmAddress), 3);
        }

        public IEnumerable<TestCaseData> EntireProjectCases()
        {
            yield return new TestCaseData(typeof(Project), 6);
        }

        [TestCaseSource("SpecificCategoryCases")]
        public void ShouldDetectSpecificCategory(Type factType, long factId)
        {
            var changesDetector = new StatisticsOperationsDetector(ErmFactsTransformationMetadata.Facts[factType], CreateFactsQuery());

            var result = changesDetector.DetectOperations(new[] { factId });

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().ProjectId, Is.EqualTo(6));
            Assert.That(result.Single().CategoryId, Is.EqualTo(4));
        }

        [TestCaseSource("EntireProjectCases")]
        public void ShouldDetectEntireProject(Type factType, long factId)
        {
            var changesDetector = new StatisticsOperationsDetector(ErmFactsTransformationMetadata.Facts[factType], CreateFactsQuery());

            var result = changesDetector.DetectOperations(new[] { factId });

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().ProjectId, Is.EqualTo(6));
            Assert.That(result.Single().CategoryId, Is.EqualTo(null));
        }

        private static IQuery CreateFactsQuery()
        {
            var query = new Mock<IQuery>();
            query.Setup(q => q.For<Project>(), new Project { Id = 6, OrganizationUnitId = 5 });
            query.Setup(q => q.For<Firm>(), q => q.For(It.IsAny<FindSpecification<Firm>>()), new Firm { Id = 1, OrganizationUnitId = 5 });
            query.Setup(q => q.For<FirmAddress>(), q => q.For(It.IsAny<FindSpecification<FirmAddress>>()), new FirmAddress { Id = 2, FirmId = 1 });
            query.Setup(q => q.For<CategoryFirmAddress>(), q => q.For(It.IsAny<FindSpecification<CategoryFirmAddress>>()), new CategoryFirmAddress { Id = 3, FirmAddressId = 2, CategoryId = 4 });
            return query.Object;
        }
    }
}
