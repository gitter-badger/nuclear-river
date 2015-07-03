using System.Collections.Generic;
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
    internal partial class WhenFirmToCategoryRelationChanged
    {
        private static readonly Mock<IErmFactsContext> Erm = new Mock<IErmFactsContext>()
            .Setup(x => x.Projects,
                   new Facts.Project { Id = 1, OrganizationUnitId = 2 },
                   new Facts.Project { Id = 3, OrganizationUnitId = 4 })
            .Setup(x => x.Categories,
                   new Facts.Category { Id = 5 },
                   new Facts.Category { Id = 6 })
            .Setup(x => x.Firms,
                   new Facts.Firm { Id = 7, OrganizationUnitId = 2 },
                   new Facts.Firm { Id = 8, OrganizationUnitId = 2 },
                   new Facts.Firm { Id = 9, OrganizationUnitId = 2 },
                   new Facts.Firm { Id = 10, OrganizationUnitId = 4 })
            .Setup(x => x.FirmAddresses,
                   new Facts.FirmAddress { Id = 7, FirmId = 7 },
                   new Facts.FirmAddress { Id = 8, FirmId = 8 },
                   new Facts.FirmAddress { Id = 9, FirmId = 9 },
                   new Facts.FirmAddress { Id = 10, FirmId = 10 })
            .Setup(x => x.CategoryFirmAddresses,
                   new Facts.CategoryFirmAddress { Id = 11, FirmAddressId = 7, CategoryId = 6 },
                   new Facts.CategoryFirmAddress { Id = 12, FirmAddressId = 8, CategoryId = 5 },
                   new Facts.CategoryFirmAddress { Id = 13, FirmAddressId = 8, CategoryId = 6 },
                   new Facts.CategoryFirmAddress { Id = 14, FirmAddressId = 9, CategoryId = 5 },
                   new Facts.CategoryFirmAddress { Id = 15, FirmAddressId = 10, CategoryId = 5 },
                   new Facts.CategoryFirmAddress { Id = 16, FirmAddressId = 10, CategoryId = 6 });

        private static IEnumerable<TestCaseData> Cases()
        {
            yield return new TestCaseData(Erm.Object, Erm.Object, new FactOperation(typeof(Facts::CategoryFirmAddress), 11));
            yield return new TestCaseData(Erm.Object, Erm.Object, new FactOperation(typeof(Facts::FirmAddress), 7));
            yield return new TestCaseData(Erm.Object, Erm.Object, new FactOperation(typeof(Facts::Firm), 7));
        }
            
        [TestCaseSource("Cases")]
        public void ItShouldAffectFirm(IErmFactsContext sourceContext, IErmFactsContext targetContext, FactOperation impact)
        {
            var transformation = new ErmFactsTransformation(sourceContext, targetContext, Mock.Of<IDataMapper>()); 

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.Member(new RecalculateAggregate(typeof(CI.Firm), 7)));
        }

        [TestCaseSource("Cases")]
        public void ItShouldAffectFirmsOfSameProjectAndSameCategory(IErmFactsContext sourceContext, IErmFactsContext targetContext, FactOperation impact)
        {
            var transformation = new ErmFactsTransformation(sourceContext, targetContext, Mock.Of<IDataMapper>()); 

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.Member(new RecalculateAggregate(typeof(CI.Firm), 8)));
        }

        [TestCaseSource("Cases")]
        public void ItShouldNotAffectFirmsOfOtherCategories(IErmFactsContext sourceContext, IErmFactsContext targetContext, FactOperation impact)
        {
            var transformation = new ErmFactsTransformation(sourceContext, targetContext, Mock.Of<IDataMapper>());

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.No.Member(new RecalculateAggregate(typeof(CI.Firm), 9)));
        }

        [TestCaseSource("Cases")]
        public void ItShouldNotAffectFirmsOfOtherProjects(IErmFactsContext sourceContext, IErmFactsContext targetContext, FactOperation impact)
        {
            var transformation = new ErmFactsTransformation(sourceContext, targetContext, Mock.Of<IDataMapper>());

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.No.Member(new RecalculateAggregate(typeof(CI.Firm), 10)));
        }
    }
}
