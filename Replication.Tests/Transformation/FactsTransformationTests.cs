using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API;
using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Readings;
using NuClear.Storage.Writings;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Facts = CustomerIntelligence.Model.Facts;
    using Erm = CustomerIntelligence.Model.Erm;

    [TestFixture]
    internal partial class FactTransformationTests : TransformationFixtureBase
    {
        [Test]
        public void ShouldInitializeClientIfClientCreated()
        {
            ErmDb.Has(new Erm::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(Aggregate.Initialize<CI.Client>(1));
        }

        [Test]
        public void ShouldRecalculateClientIfClientUpdated()
        {
            ErmDb.Has(new Erm::Client { Id = 1 });
            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Client>(1));
        }

        [Test]
        public void ShouldDestroyClientIfClientDeleted()
        {
            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(Aggregate.Destroy<CI.Client>(1));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmCreated()
        {
            ErmDb.Has(new Erm::Firm { Id = 2, ClientId = 1 });
            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(2)
                          .VerifyDistinct(op => op is RecalculateAggregate, Aggregate.Recalculate<CI.Client>(1));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmUpdated()
        {
            ErmDb.Has(new Erm::Firm { Id = 2, ClientId = 3 });

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(2), Aggregate.Recalculate<CI.Client>(3));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmDeleted()
        {
            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(2)
                          .VerifyDistinct(op => op is RecalculateAggregate, Aggregate.Recalculate<CI.Client>(1));
        }

        [Test]
        public void ShouldInitializeFirmIfFirmCreated()
        {
            ErmDb.Has(new Erm::Firm { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(1)
                          .VerifyDistinct(Aggregate.Initialize<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmUpdated()
        {
            ErmDb.Has(new Erm::Firm { Id = 1 });
            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldDestroyFirmIfFirmDeleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(1)
                          .VerifyDistinct(Aggregate.Destroy<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountCreated()
        {
            ErmDb.Has(new Erm::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            FactsDb.Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Account>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountUpdated()
        {
            ErmDb.Has(new Erm::Account { Id = 1, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::LegalPerson { Id = 2, ClientId = 2 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Account>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountDeleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Account>(5)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitCreated()
        {
            ErmDb.Has(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            FactsDb.Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitCreated()
        {
            ErmDb.Has(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            ErmDb.Has(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            ErmDb.Has(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Created()
        {
            ErmDb.Has(new Erm::Category { Id = 3, Level = 3, ParentId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Created()
        {
            ErmDb.Has(new Erm::Category { Id = 2, Level = 2, ParentId = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Created()
        {
            ErmDb.Has(new Erm::Category { Id = 1, Level = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Updated()
        {
            ErmDb.Has(new Erm::Firm { Id = 1 })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Erm::Category { Id = 1, Level = 1 },
                      new Erm::Category { Id = 2, Level = 2, ParentId = 1 },
                      new Erm::Category { Id = 3, Level = 3, ParentId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Updated()
        {
            ErmDb.Has(new Erm::Firm { Id = 1 })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Erm::Category { Id = 1, Level = 1 },
                      new Erm::Category { Id = 2, Level = 2, ParentId = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Updated()
        {
            ErmDb.Has(new Erm::Firm { Id = 1 })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Erm::Category { Id = 1, Level = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Deleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Deleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Deleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressCreated()
        {
            ErmDb.Has(new Erm::Firm { Id = 1 })
                 .Has(new Erm::FirmAddress { Id = 2, FirmId = 1 })
                 .Has(new Erm::CategoryFirmAddress { Id = 3, FirmAddressId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryFirmAddress>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressUpdated()
        {
            ErmDb.Has(new Erm::Firm { Id = 1 }, new Erm::Firm { Id = 2 })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 }, new Erm::FirmAddress { Id = 2, FirmId = 2 })
                 .Has(new Erm::CategoryFirmAddress { Id = 1, FirmAddressId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryFirmAddress>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressDeleted()
        {
            ErmDb.Has(new Erm::Firm { Id = 1 })
                 .Has(new Erm::FirmAddress { Id = 2, FirmId = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 3, FirmAddressId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryFirmAddress>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitCreated()
        {
            ErmDb.Has(new Erm::CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::FirmAddress { Id = 4, FirmId = 3 })
                   .Has(new Facts::CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryOrganizationUnit>(6)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(3));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitUpdated()
        {
            ErmDb.Has(new Erm::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 2, CategoryId = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, OrganizationUnitId = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 1 })
                   .Has(new Facts::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1, CategoryId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitDeleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::FirmAddress { Id = 4, FirmId = 3 })
                   .Has(new Facts::CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 })
                   .Has(new Facts::CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryOrganizationUnit>(6)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(3));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientCreated()
        {
            ErmDb.Has(new Erm::Client { Id = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(op => op is RecalculateAggregate, Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientUpdated()
        {
            ErmDb.Has(new Erm::Client { Id = 1 });

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Client>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientDeleted()
        {
            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(op => op is RecalculateAggregate, Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactCreated()
        {
            ErmDb.Has(new Erm::Contact { Id = 3, ClientId = 1 });

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactUpdated()
        {
            ErmDb.Has(new Erm::Contact { Id = 1, ClientId = 2 });

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2 })
                   .Has(new Facts::Contact { Id = 1, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Client>(2), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactDeleted()
        {
            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 })
                   .Has(new Facts::Contact { Id = 3, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressCreated()
        {
            ErmDb.Has(new Erm::FirmAddress { Id = 2, FirmId = 1 });
            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmAddress>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressUpdated()
        {
            ErmDb.Has(new Erm::FirmAddress { Id = 1, FirmId = 2 });
            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmAddress>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressDeleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmAddress>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactCreated()
        {
            ErmDb.Has(new Erm::FirmContact { Id = 3, FirmAddressId = 2, ContactType = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmContact>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactUpdated()
        {
            ErmDb.Has(new Erm::FirmContact { Id = 1, FirmAddressId = 2, ContactType = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::FirmContact { Id = 1, FirmAddressId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmContact>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactDeleted()
        {
            ErmDb.Has(new Erm::FirmContact { Id = 3, FirmAddressId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 })
                   .Has(new Facts::FirmContact { Id = 3, FirmAddressId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmContact>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonCreated()
        {
            ErmDb.Has((new Erm::LegalPerson { Id = 1, ClientId = 1 }));

            FactsDb.Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::LegalPerson>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonUpdated()
        {
            ErmDb.Has(new Erm::LegalPerson { Id = 1, ClientId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::LegalPerson>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonDeleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::LegalPerson>(4)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderCreated()
        {
            ErmDb.Has(new Erm::Order { Id = 2, FirmId = 1, WorkflowStepId = 4 });
            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Order>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderUpdated()
        {
            ErmDb.Has(new Erm::Order { Id = 1, FirmId = 2, WorkflowStepId = 4 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::Order { Id = 1, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Order>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderDeleted()
        {
            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Order { Id = 2, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Order>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldEnqueueOperationsInOrderForFirmIfFirmAddressCreated()
        {
            ErmDb.Has(new Erm::Firm { Id = 2 })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 }, new Erm::FirmAddress { Id = 2, FirmId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 1 });
            // Тест не пройдёт - порядок между различными типами решается уровнем выше, в Transformation
            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmAddress>(1)
                          .ApplyChanges<Facts::Firm>(2)
                          .ApplyChanges<Facts::FirmAddress>(2)
                          .VerifyDistinct(Aggregate.Initialize<CI.Firm>(2), Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        #region Transformation

        private class Transformation
        {
            private readonly IQuery _query;
            private readonly IRepositoryFactory _repositoryFactory;
            private readonly List<IOperation> _operations;
            private readonly ErmFactsTransformationMetadata _metadataSource;

            private Transformation(IQuery query, IRepositoryFactory repositoryFactory)
            {
                _query = query;
                _repositoryFactory = repositoryFactory;
                _operations = new List<IOperation>();
                _metadataSource = new ErmFactsTransformationMetadata();
            }

            public static Transformation Create(IQuery query, IRepositoryFactory repositoryFactory)
            {
                return new Transformation(query, repositoryFactory);
            }

            public Transformation ApplyChanges<TFact>(params long[] ids)
                where TFact : class, IErmFactObject
            {
                IFactInfo metadata;
                if (!_metadataSource.Metadata.TryGetValue(typeof(TFact), out metadata))
                {
                    throw new Exception(string.Format("missing metadata for fact type {0}", typeof(TFact).Name));
                }


                var repository = _repositoryFactory.Create<TFact>();
                var factory = new Factory<TFact>(_query, repository);
                var processor = factory.Create(metadata);

                _operations.AddRange(processor.ApplyChanges(ids));

                return this;
            }

            public void VerifyDistinct(params IOperation[] operations)
            {
                Assert.That(_operations.Distinct(), Is.EqualTo(operations));
            }

            public void VerifyDistinct(Func<IOperation, bool> filter, params IOperation[] operations)
            {
                Assert.That(_operations.Distinct().Where(filter), Is.EqualTo(operations));
            }

            private class Factory<TFact> : IFactProcessorFactory, IFactDependencyProcessorFactory
                where TFact : class, IErmFactObject
            {
                private readonly IQuery _query;
                private readonly IRepository<TFact> _repository;

                public Factory(IQuery query, IRepository<TFact> repository)
                {
                    _query = query;
                    _repository = repository;
                }

                public IFactProcessor Create(IFactInfo metadata)
                {
                    return new FactProcessor<TFact>((FactInfo<TFact>)metadata, this, _query, CreateBulkRepository());
                }

                public IFactDependencyProcessor Create(IFactDependencyInfo metadata)
                {
                    return new FactDependencyProcessor<TFact>((IFactDependencyInfo<TFact>)metadata, _query);
                }

                private IBulkRepository<TFact> CreateBulkRepository()
                {
                    return new BulkRepository<TFact>(_repository);
                }
            }
        }

        #endregion
    }
}