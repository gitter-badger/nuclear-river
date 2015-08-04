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
        public void ShouldRecalulateClientIfContactCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Contact>>()) == Inquire(new Erm::Contact { Id = 1, ClientId = 1 }));

            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Contact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)));
        }

        [Test]
        public void ShouldRecalulateClientIfContactDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Contact { Id = 1, ClientId = 1 })
                   .Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Contact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)));
        }

        [Test]
        public void ShouldRecalulateClientIfContactUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Contact>>()) == Inquire(new Erm::Contact { Id = 1, ClientId = 1 }));

            FactsDb.Has(new Facts::Contact { Id = 1, ClientId = 1 })
                   .Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Contact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)));
        }
    }
}