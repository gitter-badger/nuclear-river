using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Settings;

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
            var transformation = CreateTransformation();
            var operation = new FactOperation(factType, factId);

            var result = transformation.DetectStatisticsOperations(new[] { operation });

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().ProjectId, Is.EqualTo(6));
            Assert.That(result.Single().CategoryId, Is.EqualTo(4));
        }

        [TestCaseSource("EntireProjectCases")]
        public void ShouldDetectEntireProject(Type factType, long factId)
        {
            var transformation = CreateTransformation();
            var operation = new FactOperation(factType, factId);

            var result = transformation.DetectStatisticsOperations(new[] { operation });

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().ProjectId, Is.EqualTo(6));
            Assert.That(result.Single().CategoryId, Is.EqualTo(null));
        }

        private static StatisticsPrimaryTransformation CreateTransformation()
        {
            var context = new Mock<IErmFactsContext>();
            context.SetupGet(x => x.Projects).Returns(Inquire(new Project { Id = 6, OrganizationUnitId = 5 }));
            context.SetupGet(x => x.Firms).Returns(Inquire(new Firm { Id = 1, OrganizationUnitId = 5 }));
            context.SetupGet(x => x.FirmAddresses).Returns(Inquire(new FirmAddress { Id = 2, FirmId = 1 }));
            context.SetupGet(x => x.CategoryFirmAddresses).Returns(Inquire(new CategoryFirmAddress { Id = 3, FirmAddressId = 2, CategoryId = 4 }));

            var settings = new Mock<IReplicationSettings>();
            settings.SetupGet(x => x.ReplicationBatchSize).Returns(100);

            var transformation = new StatisticsPrimaryTransformation(context.Object, settings.Object);

            return transformation;
        }

        private static IQueryable<T> Inquire<T>(params T[] elements)
        {
            return elements.AsQueryable();
        }
    }
}
