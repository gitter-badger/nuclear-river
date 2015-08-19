using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Moq;

using NuClear.AdvancedSearch.Replication.API;
using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Readings;
using NuClear.Storage.Writings;

using NUnit.Framework;

using BranchOfficeOrganizationUnit = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts.BranchOfficeOrganizationUnit;
using CategoryFirmAddress = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts.CategoryFirmAddress;
using FirmAddress = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts.FirmAddress;
using LegalPerson = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts.LegalPerson;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;

    [TestFixture]
    internal partial class FactsTransformationTests : TransformationFixtureBase
    {
        [Test]
        public void ShouldInitializeClientIfClientCreated()
        {
            ErmDb.Has(new Client { Id = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Client>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI.Client>(1)));
        }

        [Test]
        public void ShouldRecalculateClientIfClientUpdated()
        {
            ErmDb.Has(new Client { Id = 1 });
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 });
            
            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Client>(1)));
        }

        [Test]
        public void ShouldDestroyClientIfClientDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Client>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI.Client>(1)));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmCreated()
        {
            ErmDb.Has(new Firm { Id = 2, ClientId = 1 });
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Firm>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Client>(1)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldRecalculateClientIfFirmUpdated()
        {
            ErmDb.Has(new Firm { Id = 2, ClientId = 3 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Firm>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(2), Aggregate.Recalculate<CI.Client>(3)));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Firm>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Client>(1)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldInitializeFirmIfFirmCreated()
        {
            ErmDb.Has(new Firm { Id = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Firm>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmUpdated()
        {
            ErmDb.Has(new Firm { Id = 1 });
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Firm>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldDestroyFirmIfFirmDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Firm>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountCreated()
        {
            ErmDb.Has(new Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            FactsDb.Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Account>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountUpdated()
        {
            ErmDb.Has(new Account { Id = 1, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 2 })
                   .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new LegalPerson { Id = 2, ClientId = 2 })
                   .Has(new CustomerIntelligence.Model.Facts.Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Account>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 2 })
                   .Has(new BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new CustomerIntelligence.Model.Facts.Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Account>(5))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitCreated()
        {
            ErmDb.Has(new CustomerIntelligence.Model.Erm.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<BranchOfficeOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitCreated()
        {
            ErmDb.Has(new CustomerIntelligence.Model.Erm.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<BranchOfficeOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            ErmDb.Has(new CustomerIntelligence.Model.Erm.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<BranchOfficeOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            ErmDb.Has(new CustomerIntelligence.Model.Erm.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                   .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<BranchOfficeOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 2 })
                   .Has(new BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new CustomerIntelligence.Model.Facts.Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<BranchOfficeOrganizationUnit>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<BranchOfficeOrganizationUnit>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Created()
        {
            ErmDb.Has(new Category { Id = 3, Level = 3, ParentId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 1, Level = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 2, Level = 2, ParentId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Category>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Created()
        {
            ErmDb.Has(new Category { Id = 2, Level = 2, ParentId = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 1, Level = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Category>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Created()
        {
            ErmDb.Has(new Category { Id = 1, Level = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Category>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Updated()
        {
            ErmDb.Has(new Firm { Id = 1 })
                 .Has(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Category { Id = 1, Level = 1 },
                      new Category { Id = 2, Level = 2, ParentId = 1 },
                      new Category { Id = 3, Level = 3, ParentId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 1, Level = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Category>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Updated()
        {
            ErmDb.Has(new Firm { Id = 1 })
                 .Has(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Category { Id = 1, Level = 1 },
                      new Category { Id = 2, Level = 2, ParentId = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 1, Level = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Category>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Updated()
        {
            ErmDb.Has(new Firm { Id = 1 })
                 .Has(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Category { Id = 1, Level = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 1, Level = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Category>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Deleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 1, Level = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Category>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Deleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 1, Level = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Category>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Deleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 1, Level = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Category>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressCreated()
        {
            ErmDb.Has(new Firm { Id = 1 })
                 .Has(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 2, FirmId = 1 })
                 .Has(new CustomerIntelligence.Model.Erm.CategoryFirmAddress { Id = 3, FirmAddressId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CategoryFirmAddress>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressUpdated()
        {
            ErmDb.Has(new Firm { Id = 1 }, new Firm { Id = 2 })
                 .Has(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 1, FirmId = 1 }, new CustomerIntelligence.Model.Erm.FirmAddress { Id = 2, FirmId = 2 })
                 .Has(new CustomerIntelligence.Model.Erm.CategoryFirmAddress { Id = 1, FirmAddressId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CategoryFirmAddress>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressDeleted()
        {
            ErmDb.Has(new Firm { Id = 1 })
                 .Has(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 2, FirmId = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 2, FirmId = 1 })
                   .Has(new CategoryFirmAddress { Id = 3, FirmAddressId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CategoryFirmAddress>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitCreated()
        {
            ErmDb.Has(new CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 3, OrganizationUnitId = 1 })
                   .Has(new FirmAddress { Id = 4, FirmId = 3 })
                   .Has(new CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.CategoryOrganizationUnit>(6))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(3)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitUpdated()
        {
            ErmDb.Has(new CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 2, CategoryId = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, OrganizationUnitId = 2 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                   .Has(new CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1, CategoryId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.CategoryOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 3, OrganizationUnitId = 1 })
                   .Has(new FirmAddress { Id = 4, FirmId = 3 })
                   .Has(new CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 })
                   .Has(new CustomerIntelligence.Model.Facts.CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.CategoryOrganizationUnit>(6))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(3)));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientCreated()
        {
            ErmDb.Has(new Client { Id = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldRecalculateFirmIfClientUpdated()
        {
            ErmDb.Has(new Client { Id = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Client>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(2)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldRecalculateFirmIfContactCreated()
        {
            ErmDb.Has(new Contact { Id = 3, ClientId = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Contact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactUpdated()
        {
            ErmDb.Has(new Contact { Id = 1, ClientId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 2 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, ClientId = 2 })
                   .Has(new CustomerIntelligence.Model.Facts.Contact { Id = 1, ClientId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Contact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Client>(2),  Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, ClientId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Contact { Id = 3, ClientId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Contact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressCreated()
        {
            ErmDb.Has(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 2, FirmId = 1 });
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<FirmAddress>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressUpdated()
        {
            ErmDb.Has(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 1, FirmId = 2 });
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<FirmAddress>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<FirmAddress>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactCreated()
        {
            ErmDb.Has(new FirmContact { Id = 3, FirmAddressId = 2, ContactType = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.FirmContact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactUpdated()
        {
            ErmDb.Has(new FirmContact { Id = 1, FirmAddressId = 2, ContactType = 1 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new CustomerIntelligence.Model.Facts.FirmContact { Id = 1, FirmAddressId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.FirmContact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactDeleted()
        {
            ErmDb.Has(new FirmContact { Id = 3, FirmAddressId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 2, FirmId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.FirmContact { Id = 3, FirmAddressId = 2 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.FirmContact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonCreated()
        {
            ErmDb.Has((new CustomerIntelligence.Model.Erm.LegalPerson { Id = 1, ClientId = 1 }));

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<LegalPerson>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonUpdated()
        {
            ErmDb.Has(new CustomerIntelligence.Model.Erm.LegalPerson { Id = 1, ClientId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 2 })
                   .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<LegalPerson>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Client { Id = 2 })
                   .Has(new BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new CustomerIntelligence.Model.Facts.Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<LegalPerson>(4))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderCreated()
        {
            ErmDb.Has(new Order { Id = 2, FirmId = 1, WorkflowStepId = 4 });
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Order>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderUpdated()
        {
            ErmDb.Has(new Order { Id = 1, FirmId = 2, WorkflowStepId = 4 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Firm { Id = 2 })
                   .Has(new CustomerIntelligence.Model.Facts.Order { Id = 1, FirmId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Order>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderDeleted()
        {
            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 })
                   .Has(new CustomerIntelligence.Model.Facts.Order { Id = 2, FirmId = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<CustomerIntelligence.Model.Facts.Order>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI.Firm>(1)));
        }

        [Test]
        public void ShouldEnqueueOperationsInOrderForFirmIfFirmAddressCreated()
        {
            ErmDb.Has(new Firm { Id = 2 })
                 .Has(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 1, FirmId = 1 }, new CustomerIntelligence.Model.Erm.FirmAddress { Id = 2, FirmId = 2 });

            FactsDb.Has(new CustomerIntelligence.Model.Facts.Firm { Id = 1 });

            Transformation.Create(Query, FactChangesApplierFactory)
                          .Transform(Fact.Operation<FirmAddress>(1), Fact.Operation<CustomerIntelligence.Model.Facts.Firm>(2), Fact.Operation<FirmAddress>(2))
                          .Verify(Inquire(Aggregate.Initialize<CI.Firm>(2), Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2)));
        }

        [TestCaseSource("Cases")]
        public void ShouldProcessChanges(Action<IQuery, MockLinqToDbDataBuilder, MockLinqToDbDataBuilder> run)
        {
            run(Query, ErmDb, FactsDb);
        }

        private IEnumerable Cases
        {
            get
            {
                const int NotNull = 1;

                // insert
                yield return CaseToVerifyElementInsertion<Account, CustomerIntelligence.Model.Facts.Account>(new Account { Id = 1 });
                yield return CaseToVerifyElementInsertion<CustomerIntelligence.Model.Erm.BranchOfficeOrganizationUnit, BranchOfficeOrganizationUnit>(new CustomerIntelligence.Model.Erm.BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementInsertion<Category, CustomerIntelligence.Model.Facts.Category>(new Category { Id = 1 });
                yield return CaseToVerifyElementInsertion<CustomerIntelligence.Model.Erm.CategoryFirmAddress, CategoryFirmAddress>(new CustomerIntelligence.Model.Erm.CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<CategoryGroup, CustomerIntelligence.Model.Facts.CategoryGroup>(new CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementInsertion<CategoryOrganizationUnit, CustomerIntelligence.Model.Facts.CategoryOrganizationUnit>(new CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementInsertion<Client, CustomerIntelligence.Model.Facts.Client>(new Client { Id = 1 });
                yield return CaseToVerifyElementInsertion<Contact, CustomerIntelligence.Model.Facts.Contact>(new Contact { Id = 1 });
                yield return CaseToVerifyElementInsertion<Firm, CustomerIntelligence.Model.Facts.Firm>(new Firm { Id = 1 });
                yield return CaseToVerifyElementInsertion<CustomerIntelligence.Model.Erm.FirmAddress, FirmAddress>(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<FirmContact, CustomerIntelligence.Model.Facts.FirmContact>(new FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull });
                yield return CaseToVerifyElementInsertion<CustomerIntelligence.Model.Erm.LegalPerson, LegalPerson>(new CustomerIntelligence.Model.Erm.LegalPerson { Id = 1, ClientId = NotNull });
                yield return CaseToVerifyElementInsertion<Order, CustomerIntelligence.Model.Facts.Order>(new Order { Id = 1, WorkflowStepId = 4 });
                yield return CaseToVerifyElementInsertion<Project, CustomerIntelligence.Model.Facts.Project>(new Project { Id = 1, OrganizationUnitId = NotNull });
                yield return CaseToVerifyElementInsertion<Territory, CustomerIntelligence.Model.Facts.Territory>(new Territory { Id = 1 });

                // update
                yield return CaseToVerifyElementUpdate(new Account { Id = 1 }, new CustomerIntelligence.Model.Facts.Account { Id = 1 });
                yield return CaseToVerifyElementUpdate(new CustomerIntelligence.Model.Erm.BranchOfficeOrganizationUnit { Id = 1 }, new BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Category { Id = 1 }, new CustomerIntelligence.Model.Facts.Category { Id = 1 });
                yield return CaseToVerifyElementUpdate(new CustomerIntelligence.Model.Erm.CategoryFirmAddress { Id = 1 }, new CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementUpdate(new CategoryGroup { Id = 1 }, new CustomerIntelligence.Model.Facts.CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementUpdate(new CategoryOrganizationUnit { Id = 1 }, new CustomerIntelligence.Model.Facts.CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Client { Id = 1 }, new CustomerIntelligence.Model.Facts.Client { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Contact { Id = 1 }, new CustomerIntelligence.Model.Facts.Contact { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Firm { Id = 1 }, new CustomerIntelligence.Model.Facts.Firm { Id = 1 });
                yield return CaseToVerifyElementUpdate(new CustomerIntelligence.Model.Erm.FirmAddress { Id = 1 }, new FirmAddress { Id = 1 });
                yield return CaseToVerifyElementUpdate(new FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull }, new CustomerIntelligence.Model.Facts.FirmContact { Id = 1, HasPhone = true, FirmAddressId = NotNull });
                yield return CaseToVerifyElementUpdate(new CustomerIntelligence.Model.Erm.LegalPerson { Id = 1, ClientId = NotNull }, new LegalPerson { Id = 1, ClientId = NotNull });
                yield return CaseToVerifyElementUpdate(new Order { Id = 1, WorkflowStepId = 4 }, new CustomerIntelligence.Model.Facts.Order { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Project { Id = 1, OrganizationUnitId = NotNull }, new CustomerIntelligence.Model.Facts.Project { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Territory { Id = 1 }, new CustomerIntelligence.Model.Facts.Territory { Id = 1 });

                // delete
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.Account { Id = 1 });
                yield return CaseToVerifyElementDeletion(new BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.Category { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.Client { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.Contact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.Firm { Id = 1 });
                yield return CaseToVerifyElementDeletion(new FirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.FirmContact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new LegalPerson { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.Order { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.Project { Id = 1 });
                yield return CaseToVerifyElementDeletion(new CustomerIntelligence.Model.Facts.Territory { Id = 1 });
            }
        }

        private static TestCaseData CaseToVerifyElementInsertion<TSource, TFactElement>(TSource sourceObject) where TSource : class, IIdentifiable, new()
            where TFactElement : class, IIdentifiable, new()
        {
            return Case((query, ermDb, factsDb) => VerifyElementInsertion<TSource, TFactElement>(query, ermDb, sourceObject))
                .SetName(string.Format("Should process and insert {0} element.", typeof(TFactElement).Name));
        }

        private static TestCaseData CaseToVerifyElementUpdate<TSource, TTarget>(TSource sourceObject, TTarget target) where TSource : class, IIdentifiable, new()
            where TTarget : class, IIdentifiable, new()
        {
            return Case((query, ermDb, factsDb) => VerifyElementUpdate(query, ermDb, factsDb, sourceObject, target))
                .SetName(string.Format("Should process and update {0} element.", typeof(TTarget).Name));
        }

        private static TestCaseData CaseToVerifyElementDeletion<TTarget>(TTarget targetObject) where TTarget : class, IIdentifiable, new()
        {
            return Case((query, ermDb, factsDb) => VerifyElementDeletion(query, factsDb, targetObject))
                .SetName(string.Format("Should process and delete {0} element.", typeof(TTarget).Name));
        }

        private static void VerifyElementInsertion<TSource, TTarget>(IQuery query, MockLinqToDbDataBuilder ermDb, TSource sourceObject) 
            where TSource : class, IIdentifiable, new()
            where TTarget : class, IIdentifiable, new()
        {
            var entityId = sourceObject.Id;
            ermDb.Has(sourceObject);

            Transformation.Create(query)
                          .Transform(Fact.Operation<TTarget>(entityId))
                          .Verify<TTarget>(
                              x => x.AddRange(It.Is(Predicate.ByIds<TTarget>(new[] { entityId }))),
                              Times.Once,
                              string.Format("The {0} element was not inserted.", typeof(TTarget).Name));
        }

        private static void VerifyElementUpdate<TSource, TTarget>(IQuery query, MockLinqToDbDataBuilder ermDb, MockLinqToDbDataBuilder factsDb, TSource sourceObject, TTarget targetObject) 
            where TSource : class, IIdentifiable, new()
            where TTarget : class, IIdentifiable, new()
        {
            ermDb.Has(sourceObject);
            factsDb.Has(targetObject);

            Transformation.Create(query)
                          .Transform(Fact.Operation<TTarget>(targetObject.Id))
                          .Verify<TTarget>(
                              x => x.Update(It.Is(Predicate.ById<TTarget>(targetObject.Id))),
                              Times.Once,
                              string.Format("The {0} element was not updated.", typeof(TTarget).Name));
        }

        private static void VerifyElementDeletion<TTarget>(IQuery query, MockLinqToDbDataBuilder factsDb, TTarget targetObject) 
            where TTarget : class, IIdentifiable, new()
        {
            factsDb.Has(targetObject);

            Transformation.Create(query)
                          .Transform(Fact.Operation<TTarget>(targetObject.Id))
                          .Verify<TTarget>(
                              x => x.DeleteRange(It.Is(Predicate.ByIds<TTarget>(new[] { targetObject.Id }))),
                              Times.Once,
                              string.Format("The {0} element was not deleted.", typeof(TTarget).Name));
        }

        private static TestCaseData Case(Action<IQuery, MockLinqToDbDataBuilder, MockLinqToDbDataBuilder> action)
        {
            return new TestCaseData(action);
        }

        #region Transformation

        private class Transformation
        {
            private static IRepository _repositoryToVerify;
            
            private readonly ErmFactsTransformation _transformation;
            private readonly List<IOperation> _operations = new List<IOperation>();

            private Transformation(IQuery query, IFactChangesApplierFactory factChangesApplierFactory)
            {
                
                _transformation = new ErmFactsTransformation(query, factChangesApplierFactory);
            }

            private Transformation(IQuery query)
                : this(query, new VerifiableFactChangesApplierFactory(OnRepositoryCreated))
            {
            }

            public static Transformation Create(IQuery query)
            {
                return new Transformation(query);
            }

            public static Transformation Create(IQuery query, IFactChangesApplierFactory factChangesApplierFactory)
            {
                return new Transformation(query, factChangesApplierFactory);
            }

            public Transformation Transform(params FactOperation[] operations)
            {
                _operations.AddRange(_transformation.Transform(operations));
                return this;
            }

            public Transformation Verify<T>(Expression<Action<IRepository<T>>> action, Func<Times> times = null, string failMessage = null) where T : class
            {
                var repository = (IRepository<T>)_repositoryToVerify;
                Mock.Get(repository)
                    .Verify(action, times ?? Times.AtLeastOnce, failMessage);
                return this;
            }

            public Transformation Verify(IEnumerable<IOperation> expected, Func<IOperation, bool> predicate = null)
            {
                var operations = _operations.AsEnumerable();
                if (predicate != null)
                {
                    operations = operations.Where(predicate);
                }

                Assert.That(operations.ToArray(), Is.EqualTo(expected.ToArray()));
                return this;
            }

            private static void OnRepositoryCreated(IRepository repository)
            {
                _repositoryToVerify = repository;
            }
        }

        #endregion
    }
}