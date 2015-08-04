using Moq;

using NuClear.AdvancedSearch.Replication.Tests.Data;
using NuClear.AdvancedSearch.Replication.Transforming;
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
        public void ShouldInitializeProjectIfProjectCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Project>>()) == Inquire(new Erm::Project { Id = 1, OrganizationUnitId = 2 }));

            Transformation.Create(source, FactsQuery)
                          .Transform(Fact.Operation<Facts::Project>(1))
                          .Verify(Inquire<IOperation>(Statistics.Operation(1), Aggregate.Initialize<CI::Project>(1)));
        }

        [Test]
        public void ShouldDestroyProjectIfProjectDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Project>(1))
                          .Verify(Inquire<IOperation>(Statistics.Operation(1), Aggregate.Destroy<CI::Project>(1)));
        }

        [Test]
        public void ShouldRecalculateDependentAggregatesIfProjectUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Project>>()) == Inquire(new Erm::Project { Id = 1, OrganizationUnitId = 2 }));

            FactsDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Territory { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Territory { Id = 2, OrganizationUnitId = 2 })
                   .Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, OrganizationUnitId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Project>(1))
                          .Verify(Inquire<IOperation>(Statistics.Operation(1),
                                                      Aggregate.Recalculate<CI::Territory>(1),
                                                      Aggregate.Recalculate<CI::Firm>(1),
                                                      Aggregate.Recalculate<CI::Project>(1),
                                                      Aggregate.Recalculate<CI::Territory>(2),
                                                      Aggregate.Recalculate<CI::Firm>(2)));
        }
    }
}