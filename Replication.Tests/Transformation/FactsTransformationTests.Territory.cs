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
            ErmDb.Has(new Erm::Territory { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<Facts::Territory>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::Territory>(1)));
        }

        [Test]
        public void ShouldDestroyTerritoryIfTerritoryDeleted()
        {
            FactsDb.Has(new Facts::Territory { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<Facts::Territory>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::Territory>(1)));
        }

        [Test]
        public void ShouldRecalculateTerritoryIfTerritoryUpdated()
        {
            ErmDb.Has(new Erm::Territory { Id = 1, OrganizationUnitId = 2 });
            FactsDb.Has(new Facts::Territory { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<Facts::Territory>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Territory>(1)));
        }
    }
}