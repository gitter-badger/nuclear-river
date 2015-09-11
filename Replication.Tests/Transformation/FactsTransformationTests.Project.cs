using NuClear.AdvancedSearch.Replication.API;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal partial class FactTransformationTests
    {
        [Test]
        public void ShouldInitializeProjectIfProjectCreated()
        {
            ErmDb.Has(new Erm::Project { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Project>(1)
                          .VerifyDistinct(Statistics.Operation(1),
                                          Aggregate.Initialize<CI::Project>(1));
        }

        [Test]
        public void ShouldDestroyProjectIfProjectDeleted()
        {
            FactsDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Project>(1)
                          .VerifyDistinct(Statistics.Operation(1),
                                          Aggregate.Destroy<CI::Project>(1));
        }

        [Test]
        public void ShouldRecalculateDependentAggregatesIfProjectUpdated()
        {
            ErmDb.Has(new Erm::Project { Id = 1, OrganizationUnitId = 2 });

            FactsDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Territory { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Territory { Id = 2, OrganizationUnitId = 2 })
                   .Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, OrganizationUnitId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Project>(1)
                          .VerifyDistinct(Statistics.Operation(1),
                                          Aggregate.Recalculate<CI::Territory>(1),
                                          Aggregate.Recalculate<CI::Firm>(1),
                                          Aggregate.Recalculate<CI::Project>(1),
                                          Aggregate.Recalculate<CI::Territory>(2),
                                          Aggregate.Recalculate<CI::Firm>(2));
        }
    }
}