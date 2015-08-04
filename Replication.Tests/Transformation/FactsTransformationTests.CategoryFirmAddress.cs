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
        public void ShouldRecalculateClientAndFirmIfCategoryFirmAddressUpdated()
        {
            var source = Mock.Of<IQuery>(query =>
                query.For(It.IsAny<FindSpecification<Erm::CategoryFirmAddress>>()) == Inquire(new Erm::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 }) &&
                query.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()) == Inquire(new Erm::FirmAddress { Id = 1, FirmId = 1 }) &&
                query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(new Erm::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 }) &&
                query.For(It.IsAny<FindSpecification<Erm::Client>>()) == Inquire(new Erm::Client { Id = 1 }));

            FactsDb.Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 });
            FactsDb.Has(new Facts::FirmAddress { Id = 1, FirmId = 1 });
            FactsDb.Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 });
            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::CategoryFirmAddress>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1),
                                          Aggregate.Recalculate<CI::Client>(1)));
        }
    }
}