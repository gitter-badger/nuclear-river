using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal partial class FactDependencyTests
    {
        [Test]
        public void ShouldRecalculateClientAndFirmIfFirmAddressUpdated()
        {
            SourceDb.Has(new Erm::FirmAddress { Id = 1, FirmId = 1 })
                .Has(new Erm::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                .Has(new Erm::Client { Id = 1 });

            TargetDb.Has(new Facts::FirmAddress { Id = 1, FirmId = 1 });
            TargetDb.Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 });
            TargetDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmAddress>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI::Firm>(1),
                                          Aggregate.Recalculate<CI::Client>(1));
        }
    }
}