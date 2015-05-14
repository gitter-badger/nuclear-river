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
        public void ShouldRecalulateClientIfContactCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Contacts == Inquire(new Facts::Contact { Id = 1, ClientId = 1 }));

            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Contact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)));
        }

        [Test]
        public void ShouldRecalulateClientIfContactDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Contact { Id = 1, ClientId = 1 })
                   .Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Contact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)));
        }

        [Test]
        public void ShouldRecalulateClientIfContactUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Contacts == Inquire(new Facts::Contact { Id = 1, ClientId = 1 }));

            FactsDb.Has(new Facts::Contact { Id = 1, ClientId = 1 })
                   .Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Contact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1), Aggregate.Recalculate<CI::Client>(1)));
        }
    }
}