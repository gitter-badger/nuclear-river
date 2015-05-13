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
        public void ShouldInitializeProjectIfProjectCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Projects == Inquire(new Facts::Project { Id = 1, OrganizationUnitId = 2 }));

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Project>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::Project>(1)));
        }

        [Test]
        public void ShouldDestroyProjectIfProjectDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Project>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::Project>(1)));
        }

        [Test]
        public void ShouldRecalculateDependentAggregatesIfProjectUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Projects == Inquire(new Facts::Project { Id = 1, OrganizationUnitId = 2 }));

            FactsDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Territory { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Territory { Id = 2, OrganizationUnitId = 2 })
                   .Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, OrganizationUnitId = 2 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Project>(1))
                          .VerifyUnordered(Inquire(Aggregate.Recalculate<CI::Territory>(1),
                                                   Aggregate.Recalculate<CI::Territory>(2),
                                                   Aggregate.Recalculate<CI::Firm>(1),
                                                   Aggregate.Recalculate<CI::Firm>(2),
                                                   Aggregate.Recalculate<CI::Project>(1)));
        }
    }
}