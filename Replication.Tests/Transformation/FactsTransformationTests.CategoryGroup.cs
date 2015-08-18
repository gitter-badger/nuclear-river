using Moq;

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
    internal partial class FactsTransformationTests
    {
        [Test]
        public void ShouldInitializeCategoryGroupIfCategoryGroupCreated()
        {
            // Erm
            var query = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Erm::CategoryGroup>>()) == Inquire(new Erm::CategoryGroup { Id = 1, Name = "Name", Rate = 1 }));

            Transformation.Create(query)
                          .Transform(Fact.Operation<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::CategoryGroup>(1)));
        }

        [Test]
        public void ShouldDestroyCategoryGroupIfCategoryGroupDeleted()
        {
            // Facts
            var query = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Facts::CategoryGroup>>()) == Inquire(new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 1 }));

            Transformation.Create(query)
                          .Transform(Fact.Operation<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::CategoryGroup>(1)));
        }

        [Test]
        public void ShouldRecalculateCategoryGroupIfCategoryGroupUpdated()
        {
            var query = new Mock<IQuery>();

            // Erm
            query.Setup(q => q.For(It.IsAny<FindSpecification<Erm::CategoryGroup>>()), new Erm::CategoryGroup { Id = 1, Name = "FooBar", Rate = 2 });

            // Facts
            query.Setup(q => q.For(It.IsAny<FindSpecification<Facts::CategoryGroup>>()), new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Transformation.Create(query.Object)
                          .Transform(Fact.Operation<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::CategoryGroup>(1)));
        }

        [Test]
        public void ShouldRecalculateClientAndFirmIfCategoryGroupUpdated()
        {
            var query = new Mock<IQuery>();

            // Erm
            query.Setup(q => q.For(It.IsAny<FindSpecification<Erm::CategoryGroup>>()), new Erm::CategoryGroup { Id = 1, Name = "Name", Rate = 1 })
                 .Setup(q => q.For(It.IsAny<FindSpecification<Erm::CategoryOrganizationUnit>>()), new Erm::CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 })
                 .Setup(q => q.For(It.IsAny<FindSpecification<Erm::CategoryFirmAddress>>()), new Erm::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                 .Setup(q => q.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()), new Erm::FirmAddress { Id = 1, FirmId = 1 })
                 .Setup(q => q.For(It.IsAny<FindSpecification<Erm::Firm>>()), new Erm::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                 .Setup(q => q.For(It.IsAny<FindSpecification<Erm::Client>>()), new Erm::Client { Id = 1 });

            // Facts
            query.Setup(q => q.For(It.IsAny<FindSpecification<Facts::CategoryGroup>>()), new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 1 })
                 .Setup(q => q.For<Facts::CategoryOrganizationUnit>(), new Facts::CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 })
                 .Setup(q => q.For<Facts::CategoryFirmAddress>(), new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                 .Setup(q => q.For<Facts::FirmAddress>(), new Facts::FirmAddress { Id = 1, FirmId = 1 })
                 .Setup(q => q.For<Facts::Firm>(), new Facts::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                 .Setup(q => q.For<Facts::Client>(), new Facts::Client { Id = 1 });

            Transformation.Create(query.Object)
                          .Transform(Fact.Operation<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1),
                                          Aggregate.Recalculate<CI::Client>(1),
                                          Aggregate.Recalculate<CI::CategoryGroup>(1)));
        }
    }
}