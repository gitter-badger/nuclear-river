using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;
using NuClear.AdvancedSearch.Replication.Tests.Data;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;
    using CI = CustomerIntelligence.Model;

    [TestFixture]
    internal class FactsTransformationTests : BaseTransformationFixture
    {
        [Test]
        public void ShouldInitializeClientIfClientCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Clients == Inquire(new Facts::Client { Id = 1 }));

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::Client>(1)));
        }

        [Test]
        public void ShouldRecalculateClientIfClientUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Clients == Inquire(new Facts::Client { Id = 1 }));
            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)));
        }

        [Test]
        public void ShouldDestroyClientIfClientDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::Client>(1)));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Firms == Inquire(new Facts::Firm { Id = 2, ClientId = 1 }));

            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Firm>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldRecalculateClientIfFirmUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Firms == Inquire(new Facts::Firm { Id = 2, ClientId = 3 }));

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Firm>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1), Aggregate.Recalculate<CI::Firm>(2), Aggregate.Recalculate<CI::Client>(3)));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Firm>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldInitializeFirmIfFirmCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Firms == Inquire(new Facts::Firm { Id = 1 }));

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Firm>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Firms == Inquire(new Facts::Firm { Id = 1 }));
            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Firm>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldDestroyFirmIfFirmDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Firm>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Accounts == Inquire(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 }));

            FactsDb.Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Account>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Accounts == Inquire(new Facts::Account { Id = 1, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::LegalPerson { Id = 2, ClientId = 2 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Account>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Account>(5))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.BranchOfficeOrganizationUnits == Inquire(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 }));

            FactsDb.Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::BranchOfficeOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.BranchOfficeOrganizationUnits == Inquire(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::BranchOfficeOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::BranchOfficeOrganizationUnit>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.CategoryFirmAddresses == Inquire(new Facts::CategoryFirmAddress { Id = 3, FirmAddressId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::CategoryFirmAddress>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.CategoryFirmAddresses == Inquire(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::CategoryFirmAddress>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1),Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 3, FirmAddressId = 2 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::CategoryFirmAddress>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitCreated()
        {
            var source =
                Mock.Of<IFactsContext>(ctx => ctx.CategoryOrganizationUnits == Inquire(new Facts::CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::FirmAddress { Id = 4, FirmId = 3 })
                   .Has(new Facts::CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::CategoryOrganizationUnit>(6))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(3)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitUpdated()
        {
            var source =
                Mock.Of<IFactsContext>(ctx => ctx.CategoryOrganizationUnits == Inquire(new Facts::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 2, CategoryId = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, OrganizationUnitId = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 1 })
                   .Has(new Facts::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1, CategoryId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::CategoryOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Firm { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::FirmAddress { Id = 4, FirmId = 3 })
                   .Has(new Facts::CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 })
                   .Has(new Facts::CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::CategoryOrganizationUnit>(6))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(3)));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Clients == Inquire(new Facts::Client { Id = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldRecalculateFirmIfClientUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Clients == Inquire(new Facts::Client { Id = 1 }));

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Client>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(2)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldRecalculateFirmIfContactCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Contacts == Inquire(new Facts::Contact { Id = 3, ClientId = 1 }));

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Contact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Contacts == Inquire(new Facts::Contact { Id = 1, ClientId = 2 }));

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2 })
                   .Has(new Facts::Contact { Id = 1, ClientId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Contact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1),Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 })
                   .Has(new Facts::Contact { Id = 3, ClientId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Contact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.FirmAddresses == Inquire(new Facts::FirmAddress { Id = 2, FirmId = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::FirmAddress>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.FirmAddresses == Inquire(new Facts::FirmAddress { Id = 1, FirmId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::FirmAddress>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::FirmAddress>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.FirmContacts == Inquire(new Facts::FirmContact { Id = 3, FirmAddressId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::FirmContact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.FirmContacts == Inquire(new Facts::FirmContact { Id = 1, FirmAddressId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::FirmContact { Id = 1, FirmAddressId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::FirmContact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactDeleted()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.FirmContacts == Inquire(new Facts::FirmContact { Id = 3, FirmAddressId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 })
                   .Has(new Facts::FirmContact { Id = 3, FirmAddressId = 2 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::FirmContact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.LegalPersons == Inquire(new Facts::LegalPerson { Id = 1, ClientId = 1 }));

            FactsDb.Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::LegalPerson>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.LegalPersons == Inquire(new Facts::LegalPerson { Id = 1, ClientId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::LegalPerson>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::LegalPerson>(4))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Orders == Inquire(new Facts::Order { Id = 2, FirmId = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Order>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.Orders == Inquire(new Facts::Order { Id = 1, FirmId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::Order { Id = 1, FirmId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Order>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Order { Id = 2, FirmId = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Order>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [TestCaseSource("Cases")]
        public void ShouldProcessChanges(Action test)
        {
            test();
        }

        private IEnumerable Cases
        {
            get
            {
                const int notnull = 1;

                // insert
                yield return CaseToVerifyElementInsertion<Erm::Account, Facts::Account>(new Erm::Account { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::BranchOfficeOrganizationUnit, Facts::BranchOfficeOrganizationUnit>(new Erm::BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::CategoryFirmAddress, Facts::CategoryFirmAddress>(new Erm::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::CategoryOrganizationUnit, Facts::CategoryOrganizationUnit>(new Erm::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::Client, Facts::Client>(new Erm::Client { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::Contact, Facts::Contact>(new Erm::Contact { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::Firm, Facts::Firm>(new Erm::Firm { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::FirmAddress, Facts::FirmAddress>(new Erm::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::FirmContact, Facts::FirmContact>(new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = notnull });
                yield return CaseToVerifyElementInsertion<Erm::LegalPerson, Facts::LegalPerson>(new Erm::LegalPerson { Id = 1, ClientId = notnull });
                yield return CaseToVerifyElementInsertion<Erm::Order, Facts::Order>(new Erm::Order { Id = 1, WorkflowStepId = 4 });
                // update
                yield return CaseToVerifyElementUpdate(new Erm::Account { Id = 1 }, new Facts::Account { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::BranchOfficeOrganizationUnit { Id = 1 }, new Facts::BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::CategoryFirmAddress { Id = 1 }, new Facts::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::CategoryOrganizationUnit { Id = 1 }, new Facts::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Client { Id = 1 }, new Facts::Client { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Contact { Id = 1 }, new Facts::Contact { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Firm { Id = 1 }, new Facts::Firm { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::FirmAddress { Id = 1 }, new Facts::FirmAddress { Id = 1 });
                yield return
                    CaseToVerifyElementUpdate(
                        new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = notnull },
                        new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = notnull });
                yield return CaseToVerifyElementUpdate(new Erm::LegalPerson { Id = 1, ClientId = notnull }, new Facts::LegalPerson { Id = 1, ClientId = notnull });
                yield return CaseToVerifyElementUpdate(new Erm::Order { Id = 1, WorkflowStepId = 4 }, new Facts::Order { Id = 1 });
                // delete
                yield return CaseToVerifyElementDeletion(new Facts::Account { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Client { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Contact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Firm { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::FirmContact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::LegalPerson { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Order { Id = 1 });
            }
        }

        private TestCaseData CaseToVerifyElementInsertion<TErmElement, TFactElement>(TErmElement source) where TErmElement : IIdentifiableObject, new()
            where TFactElement : IIdentifiableObject, new()
        {
            return Case(() => VerifyElementInsertion<TErmElement, TFactElement>(source))
                .SetName(string.Format("Should process and insert {0} element.", typeof(TFactElement).Name));
        }

        private TestCaseData CaseToVerifyElementUpdate<TErmElement, TFactElement>(TErmElement source, TFactElement target) where TErmElement : IIdentifiableObject, new()
            where TFactElement : IIdentifiableObject, new()
        {
            return Case(() => VerifyElementUpdate(source, target))
                .SetName(string.Format("Should process and update {0} element.", typeof(TFactElement).Name));
        }

        private TestCaseData CaseToVerifyElementDeletion<TFactElement>(TFactElement target) where TFactElement : IIdentifiableObject, new()
        {
            return Case(() => VerifyElementDeletion(target))
                .SetName(string.Format("Should process and delete {0} element.", typeof(TFactElement).Name));
        }

        private void VerifyElementInsertion<TErmElement, TFactElement>(TErmElement source) where TErmElement : IIdentifiableObject, new()
            where TFactElement : IIdentifiableObject, new()
        {
            var entityId = source.Id;
            ErmDb.Has(source);

            Transformation.Create(ErmDb, FactsDb)
                          .Transform(Fact.Create<TFactElement>(entityId))
                          .Verify(
                              x => x.Insert(It.Is(Predicate.ById<TFactElement>(entityId))),
                              Times.Once,
                              string.Format("The {0} element was not inserted.", typeof(TFactElement).Name));
        }

        private void VerifyElementUpdate<TErmElement, TFactElement>(TErmElement source, TFactElement target) where TErmElement : IIdentifiableObject, new()
            where TFactElement : IIdentifiableObject, new()
        {
            ErmDb.Has(source);
            FactsDb.Has(target);

            Transformation.Create(ErmDb, FactsDb)
                          .Transform(Fact.Update<TFactElement>(target.Id))
                          .Verify(
                              x => x.Update(It.Is(Predicate.ById<TFactElement>(target.Id))),
                              Times.Once,
                              string.Format("The {0} element was not updated.", typeof(TFactElement).Name));
        }

        private void VerifyElementDeletion<TFactElement>(TFactElement target) where TFactElement : IIdentifiableObject, new()
        {
            FactsDb.Has(target);

            Transformation.Create(ErmDb, FactsDb)
                          .Transform(Fact.Delete<TFactElement>(target.Id))
                          .Verify(
                              x => x.Delete(It.Is(Predicate.ById<TFactElement>(target.Id))),
                              Times.Once,
                              string.Format("The {0} element was not deleted.", typeof(TFactElement).Name));
        }

        #region Transformation

        private class Transformation
        {
            private readonly FactsTransformation _transformation;
            private readonly IDataMapper _mapper;
            private readonly List<AggregateOperation> _operations;

            private Transformation(IFactsContext source, IFactsContext target, IDataMapper mapper)
            {
                _mapper = mapper ?? Mock.Of<IDataMapper>();
                _transformation = new FactsTransformation(source, target, _mapper);
                _operations = new List<AggregateOperation>();
            }

            public static Transformation Create(IDataContext source = null, IDataContext target = null, IDataMapper mapper = null)
            {
                return Create(new ErmContext(source ?? new Mock<IDataContext>().Object), new FactsContext(target ?? new Mock<IDataContext>().Object), mapper);
            }

            public static Transformation Create(IErmContext source = null, IFactsContext target = null, IDataMapper mapper = null)
            {
                return Create(new FactsTransformationContext(source ?? new Mock<IErmContext>().Object), target, mapper);
            }

            public static Transformation Create(IFactsContext source = null, IFactsContext target = null, IDataMapper mapper = null)
            {
                return new Transformation(source ?? new Mock<IFactsContext>().Object, target ?? new Mock<IFactsContext>().Object, mapper);
            }

            public static Transformation Create(IFactsContext source, IDataContext target)
            {
                return new Transformation(source, new FactsContext(target), new DataMapper(target));
            }

            public Transformation Transform(params FactOperation[] operations)
            {
                _operations.AddRange(_transformation.Transform(operations));
                return this;
            }

            public Transformation Verify(Expression<Action<IDataMapper>> action, Func<Times> times = null, string failMessage = null)
            {
                Mock.Get(_mapper)
                    .Verify(action, times ?? Times.AtLeastOnce, failMessage);
                return this;
            }

            public Transformation Verify(IEnumerable<AggregateOperation> expected, Func<AggregateOperation, bool> predicate = null)
            {
                var operations = _operations.AsEnumerable();
                if (predicate != null)
                {
                    operations = operations.Where(predicate);
                }
                Assert.That(operations.ToArray(), Is.EquivalentTo(expected.ToArray()));
                return this;
            }
        }

        #endregion
    }
}