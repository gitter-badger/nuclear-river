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
        public void ShouldInitializeCategoryIfCategoryCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Category>>()) == Inquire(new Erm::Category { Id = 1 }));

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::Category>(1)));
        }

        [Test]
        public void ShouldRecalculateCategoryIfCategoryUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Category>>()) == Inquire(new Erm::Category { Id = 1 }));

            FactsDb.Has(new Facts::Category { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Category>(1)));
        }

        [Test]
        public void ShouldDestroyCategoryIfCategoryDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Category { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::Category>(1)));
        }
    }
}