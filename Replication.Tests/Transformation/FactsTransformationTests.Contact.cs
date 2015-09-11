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
        public void ShouldRecalulateClientIfContactCreated()
        {
            ErmDb.Has(new Erm::Contact { Id = 1, ClientId = 1 });

            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI::Client>(1));
        }

        [Test]
        public void ShouldRecalulateClientIfContactDeleted()
        {
            FactsDb.Has(new Facts::Contact { Id = 1, ClientId = 1 })
                   .Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI::Client>(1));
        }

        [Test]
        public void ShouldRecalulateClientIfContactUpdated()
        {
            ErmDb.Has(new Erm::Contact { Id = 1, ClientId = 1 });

            FactsDb.Has(new Facts::Contact { Id = 1, ClientId = 1 })
                   .Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI::Client>(1));
        }
    }
}