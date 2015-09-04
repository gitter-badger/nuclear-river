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
            var query = new MemoryMockQuery(
                new Erm::CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Transformation.Create(query)
                          .Transform(Fact.Operation<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::CategoryGroup>(1)));
        }

        [Test]
        public void ShouldDestroyCategoryGroupIfCategoryGroupDeleted()
        {
            // Facts
            var query = new MemoryMockQuery(
                new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Transformation.Create(query)
                          .Transform(Fact.Operation<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::CategoryGroup>(1)));
        }

        [Test]
        public void ShouldRecalculateCategoryGroupIfCategoryGroupUpdated()
        {
            var query = new MemoryMockQuery(
                new Erm::CategoryGroup { Id = 1, Name = "FooBar", Rate = 2 },
                new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Transformation.Create(query)
                          .Transform(Fact.Operation<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::CategoryGroup>(1)));
        }

        [Test]
        public void ShouldRecalculateClientAndFirmIfCategoryGroupUpdated()
        {
            var query = new MemoryMockQuery(
                new Erm::CategoryGroup { Id = 1, Name = "Name", Rate = 1 },
                new Erm::CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 },
                new Erm::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 },
                new Erm::FirmAddress { Id = 1, FirmId = 1 },
                new Erm::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 },
                new Erm::Client { Id = 1 },

                new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 1 },
                new Facts::CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 },
                new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 },
                new Facts::FirmAddress { Id = 1, FirmId = 1 },
                new Facts::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 },
                new Facts::Client { Id = 1 });

            Transformation.Create(query)
                          .Transform(Fact.Operation<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1),
                                          Aggregate.Recalculate<CI::Client>(1),
                                          Aggregate.Recalculate<CI::CategoryGroup>(1)));
        }
    }
}