using Moq;

using NuClear.AdvancedSearch.Replication.Tests.Data;
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
        public void ShouldInitializeTerritoryIfTerritoryCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Territory>>()) == Inquire(new Erm::Territory { Id = 1, OrganizationUnitId = 2 }));

            Transformation.Create(source, FactsQuery)
                          .Transform(Fact.Operation<Facts::Territory>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::Territory>(1)));
        }

        [Test]
        public void ShouldDestroyTerritoryIfTerritoryDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Territory { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Territory>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::Territory>(1)));
        }

        [Test]
        public void ShouldRecalculateTerritoryIfTerritoryUpdated()
        {
            var source = Mock.Of<IQuery>(ctx => ctx.For(It.IsAny<FindSpecification<Erm::Territory>>()) == Inquire(new Erm::Territory { Id = 1, OrganizationUnitId = 2 }));

            FactsDb.Has(new Facts::Territory { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Territory>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Territory>(1)));
        }
    }
}