using System;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.Tests.Data;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal partial class FactsTransformationTests
    {
        [Test]
        public void ShouldRecalculateFirmsIfActivityUpdated()
        {
            var source = Mock.Of<IErmFactsContext>(ctx => 
                ctx.Activities == Inquire(new Facts::Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now }));

            FactsDb.Has(new Facts::Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now });
            FactsDb.Has(new Facts::Client { Id = 2 });
            FactsDb.Has(new Facts::Firm { Id = 3 });
            FactsDb.Has(new Facts::Firm { Id = 4, ClientId = 2 });
            FactsDb.Has(new Facts::Firm { Id = 5 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Operation<Facts::Activity>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(3),
                                          Aggregate.Recalculate<CI::Firm>(4),
                                          Aggregate.Recalculate<CI::Firm>(3),
                                          Aggregate.Recalculate<CI::Firm>(4)));
        }

        [Test]
        public void ShouldRecalculateFirmsIfActivityCreated()
        {
            var source = Mock.Of<IErmFactsContext>(ctx =>
                ctx.Activities == Inquire(new Facts::Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now }));

            FactsDb.Has(new Facts::Client { Id = 2 });
            FactsDb.Has(new Facts::Firm { Id = 3 });
            FactsDb.Has(new Facts::Firm { Id = 4, ClientId = 2 });
            FactsDb.Has(new Facts::Firm { Id = 5 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Operation<Facts::Activity>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(3),
                                          Aggregate.Recalculate<CI::Firm>(4)));
        }
        [Test]
        public void ShouldRecalculateFirmsIfActivityDeleted()
        {
            var source = Mock.Of<IErmFactsContext>();

            FactsDb.Has(new Facts::Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now });
            FactsDb.Has(new Facts::Client { Id = 2 });
            FactsDb.Has(new Facts::Firm { Id = 3 });
            FactsDb.Has(new Facts::Firm { Id = 4, ClientId = 2 });
            FactsDb.Has(new Facts::Firm { Id = 5 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Operation<Facts::Activity>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(3),
                                          Aggregate.Recalculate<CI::Firm>(4)));
        }
    }
}