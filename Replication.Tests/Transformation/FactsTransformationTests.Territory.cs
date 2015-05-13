using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.Tests.Data;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;
    using CI = CustomerIntelligence.Model;

    [TestFixture]
    internal partial class FactsTransformationTests
    {
        [Test]
        public void ShouldInitializeTerritoryIfTerritoryCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Territories == Inquire(new Facts::Territory { Id = 1, OrganizationUnitId = 2 }));

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Territory>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::Territory>(1)));
        }

        [Test]
        public void ShouldDestroyTerritoryIfTerritoryDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Territory { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Territory>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::Territory>(1)));
        }

        [Test]
        public void ShouldRecalculateTerritoryIfTerritoryUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Territories == Inquire(new Facts::Territory { Id = 1, OrganizationUnitId = 2 }));

            FactsDb.Has(new Facts::Territory { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Territory>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Territory>(1)));
        }
    }
}