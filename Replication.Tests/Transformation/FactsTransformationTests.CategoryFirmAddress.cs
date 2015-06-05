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
        public void ShouldRecalculateClientAndFirmIfCategoryFirmAddressUpdated()
        {
            var source = Mock.Of<IErmFactsContext>(ctx =>
                ctx.CategoryFirmAddresses == Inquire(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 }) &&
                ctx.FirmAddresses == Inquire(new Facts::FirmAddress { Id = 1, FirmId = 1 }) &&
                ctx.Firms == Inquire(new Facts::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 }) &&
                ctx.Clients == Inquire(new Facts::Client { Id = 1 }));

            FactsDb.Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 });
            FactsDb.Has(new Facts::FirmAddress { Id = 1, FirmId = 1 });
            FactsDb.Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 });
            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::CategoryFirmAddress>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1),
                                          Aggregate.Recalculate<CI::Client>(1),
                                          Aggregate.Recalculate<CI::Firm>(1),
                                          Aggregate.Recalculate<CI::Client>(1)));
        }
    }
}