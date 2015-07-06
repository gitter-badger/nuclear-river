using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal partial class WhenFirmToCategoryRelationChanged
    {
        private static readonly Mock<IQuery> Erm = new Mock<IQuery>()
            .Setup(query => query.For(It.IsAny<FindSpecification<Erm::Project>>()),
                   new Erm.Project { Id = 1, OrganizationUnitId = 2 },
                   new Erm.Project { Id = 3, OrganizationUnitId = 4 })
            .Setup(query => query.For(It.IsAny<FindSpecification<Erm::Category>>()),
                   new Erm.Category { Id = 5 },
                   new Erm.Category { Id = 6 })
            .Setup(query => query.For(It.IsAny<FindSpecification<Erm::Firm>>()),
                   new Erm.Firm { Id = 7, OrganizationUnitId = 2 },
                   new Erm.Firm { Id = 8, OrganizationUnitId = 2 },
                   new Erm.Firm { Id = 9, OrganizationUnitId = 2 },
                   new Erm.Firm { Id = 10, OrganizationUnitId = 4 })
            .Setup(query => query.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()),
                   new Erm.FirmAddress { Id = 7, FirmId = 7 },
                   new Erm.FirmAddress { Id = 8, FirmId = 8 },
                   new Erm.FirmAddress { Id = 9, FirmId = 9 },
                   new Erm.FirmAddress { Id = 10, FirmId = 10 })
            .Setup(query => query.For(It.IsAny<FindSpecification<Erm::CategoryFirmAddress>>()),
                   new Erm.CategoryFirmAddress { Id = 11, FirmAddressId = 7, CategoryId = 6 },
                   new Erm.CategoryFirmAddress { Id = 12, FirmAddressId = 8, CategoryId = 5 },
                   new Erm.CategoryFirmAddress { Id = 13, FirmAddressId = 8, CategoryId = 6 },
                   new Erm.CategoryFirmAddress { Id = 14, FirmAddressId = 9, CategoryId = 5 },
                   new Erm.CategoryFirmAddress { Id = 15, FirmAddressId = 10, CategoryId = 5 },
                   new Erm.CategoryFirmAddress { Id = 16, FirmAddressId = 10, CategoryId = 6 });

        [TestCaseSource("Cases")]
        public void ItShouldAffectFirm(IQuery sourceQuery, IQuery targetQuery, FactOperation impact)
        {
            var transformation = new ErmFactsTransformation(sourceQuery, targetQuery, Mock.Of<IDataMapper>(), Mock.Of<ITransactionManager>()); 

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.Member(new RecalculateAggregate(typeof(CI.Firm), 7)));
        }

        [TestCaseSource("Cases")]
        public void ItShouldAffectFirmsOfSameProjectAndSameCategory(IQuery sourceQuery, IQuery targetQuery, FactOperation impact)
        {
            var transformation = new ErmFactsTransformation(sourceQuery, targetQuery, Mock.Of<IDataMapper>(), Mock.Of<ITransactionManager>()); 

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.Member(new RecalculateAggregate(typeof(CI.Firm), 8)));
        }

        [TestCaseSource("Cases")]
        public void ItShouldNotAffectFirmsOfOtherCategories(IQuery sourceQuery, IQuery targetQuery, FactOperation impact)
        {
            var transformation = new ErmFactsTransformation(sourceQuery, targetQuery, Mock.Of<IDataMapper>(), Mock.Of<ITransactionManager>());

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.No.Member(new RecalculateAggregate(typeof(CI.Firm), 9)));
        }

        [TestCaseSource("Cases")]
        public void ItShouldNotAffectFirmsOfOtherProjects(IQuery sourceQuery, IQuery targetQuery, FactOperation impact)
        {
            var transformation = new ErmFactsTransformation(sourceQuery, targetQuery, Mock.Of<IDataMapper>(), Mock.Of<ITransactionManager>());

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.No.Member(new RecalculateAggregate(typeof(CI.Firm), 10)));
        }

        private static IEnumerable<TestCaseData> Cases()
        {
            yield return new TestCaseData(Erm.Object, Erm.Object, new FactOperation(typeof(Facts::CategoryFirmAddress), 11));
            yield return new TestCaseData(Erm.Object, Erm.Object, new FactOperation(typeof(Facts::FirmAddress), 7));
            yield return new TestCaseData(Erm.Object, Erm.Object, new FactOperation(typeof(Facts::Firm), 7));
        }
    }
}
