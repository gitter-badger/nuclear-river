using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB.Data;

using Moq;

using NuClear.AdvancedSearch.Replication.API;
using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Tests.Data;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;
using NuClear.Storage.Writings;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal partial class FactsTransformationTests : BaseTransformationFixture
    {
        [Test]
        public void ShouldInitializeClientIfClientCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Client>>()) == Inquire(new Erm::Client { Id = 1 }));

            Transformation.Create(source, FactsQuery)
                          .Transform(Fact.Operation<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::Client>(1)));
        }

        [Test]
        public void ShouldRecalculateClientIfClientUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Client>>()) == Inquire(new Erm::Client { Id = 1 }));
            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsQuery)
                          .Transform(Fact.Operation<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)));
        }

        [Test]
        public void ShouldDestroyClientIfClientDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsQuery)
                          .Transform(Fact.Operation<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::Client>(1)));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(new Erm::Firm { Id = 2, ClientId = 1 }));

            FactsDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Firm>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldRecalculateClientIfFirmUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(new Erm::Firm { Id = 2, ClientId = 3 }));

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Firm>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1), Aggregate.Recalculate<CI::Firm>(2), Aggregate.Recalculate<CI::Client>(3)));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Firm>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldInitializeFirmIfFirmCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(new Erm::Firm { Id = 1 }));

            Transformation.Create(source, FactsQuery)
                          .Transform(Fact.Operation<Facts::Firm>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(new Erm::Firm { Id = 1 }));
            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Firm>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldDestroyFirmIfFirmDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Firm>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Account>>()) == Inquire(new Erm::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 }));

            FactsDb.Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Account>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Account>>()) == Inquire(new Erm::Account { Id = 1, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::LegalPerson { Id = 2, ClientId = 2 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Account>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Account>(5))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::BranchOfficeOrganizationUnit>>()) == Inquire(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 }));

            FactsDb.Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::BranchOfficeOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::BranchOfficeOrganizationUnit>>()) == Inquire(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::BranchOfficeOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::BranchOfficeOrganizationUnit>>()) == Inquire(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::BranchOfficeOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::BranchOfficeOrganizationUnit>>()) == Inquire(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::BranchOfficeOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::BranchOfficeOrganizationUnit>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::BranchOfficeOrganizationUnit>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Created()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Category>>()) == Inquire(new Erm::Category { Id = 3, Level = 3, ParentId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Created()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Category>>()) == Inquire(new Erm::Category { Id = 2, Level = 2, ParentId = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Created()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Category>>()) == Inquire(new Erm::Category { Id = 1, Level = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Updated()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For(It.IsAny<FindSpecification<Erm::Firm>>()), new Erm::Firm { Id = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()), new Erm::FirmAddress { Id = 1, FirmId = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Erm::Category>>()),
                         new Erm::Category { Id = 1, Level = 1 },
                         new Erm::Category { Id = 2, Level = 2, ParentId = 1 },
                         new Erm::Category { Id = 3, Level = 3, ParentId = 2 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(source.Object, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Updated()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For(It.IsAny<FindSpecification<Erm::Firm>>()), new Erm::Firm { Id = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()), new Erm::FirmAddress { Id = 1, FirmId = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Erm::Category>>()),
                         new Erm::Category { Id = 1, Level = 1 },
                         new Erm::Category { Id = 2, Level = 2, ParentId = 1 });

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(source.Object, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Updated()
        {
            var source = Mock.Of<IQuery>(query =>
                query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(new Erm::Firm { Id = 1 })
                && query.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()) == Inquire(new Erm::FirmAddress { Id = 1, FirmId = 1 })
                && query.For(It.IsAny<FindSpecification<Erm::Category>>()) == Inquire(new Erm::Category { Id = 1, Level = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Deleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Deleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Deleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Category>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressCreated()
        {
            var source = Mock.Of<IQuery>(query =>
                query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(new Erm::Firm { Id = 1 })
                && query.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()) == Inquire(new Erm::FirmAddress { Id = 2, FirmId = 1 })
                && query.For(It.IsAny<FindSpecification<Erm::CategoryFirmAddress>>()) == Inquire(new Erm::CategoryFirmAddress { Id = 3, FirmAddressId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::CategoryFirmAddress>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressUpdated()
        {
            var source = Mock.Of<IQuery>(query =>
                query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(new Erm::Firm { Id = 1 }, new Erm::Firm { Id = 2 })
                && query.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()) == Inquire(new Erm::FirmAddress { Id = 1, FirmId = 1 }, new Erm::FirmAddress { Id = 2, FirmId = 2 })
                && query.For(It.IsAny<FindSpecification<Erm::CategoryFirmAddress>>()) == Inquire(new Erm::CategoryFirmAddress { Id = 1, FirmAddressId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::CategoryFirmAddress>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressDeleted()
        {
            var source = Mock.Of<IQuery>(query =>
                query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(new Erm::Firm { Id = 1 })
                && query.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()) == Inquire(new Erm::FirmAddress { Id = 2, FirmId = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 3, FirmAddressId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::CategoryFirmAddress>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitCreated()
        {
            var source =
                Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::CategoryOrganizationUnit>>()) == Inquire(new Erm::CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::FirmAddress { Id = 4, FirmId = 3 })
                   .Has(new Facts::CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::CategoryOrganizationUnit>(6))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(3)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitUpdated()
        {
            var source =
                Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::CategoryOrganizationUnit>>()) == Inquire(new Erm::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 2, CategoryId = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, OrganizationUnitId = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 1 })
                   .Has(new Facts::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1, CategoryId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::CategoryOrganizationUnit>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::FirmAddress { Id = 4, FirmId = 3 })
                   .Has(new Facts::CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 })
                   .Has(new Facts::CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::CategoryOrganizationUnit>(6))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(3)));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Client>>()) == Inquire(new Erm::Client { Id = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1 });

            Transformation.Create(source, FactsQuery)
                          .Transform(Fact.Operation<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldRecalculateFirmIfClientUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Client>>()) == Inquire(new Erm::Client { Id = 1 }));

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Client>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Client>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(2)), op => op is RecalculateAggregate);
        }

        [Test]
        public void ShouldRecalculateFirmIfContactCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Contact>>()) == Inquire(new Erm::Contact { Id = 3, ClientId = 1 }));

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Contact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Contact>>()) == Inquire(new Erm::Contact { Id = 1, ClientId = 2 }));

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2 })
                   .Has(new Facts::Contact { Id = 1, ClientId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Contact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1), Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Client>(2),  Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 })
                   .Has(new Facts::Contact { Id = 3, ClientId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Contact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Client>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()) == Inquire(new Erm::FirmAddress { Id = 2, FirmId = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::FirmAddress>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()) == Inquire(new Erm::FirmAddress { Id = 1, FirmId = 2 }));
            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::FirmAddress>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::FirmAddress>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::FirmContact>>()) == Inquire(new Erm::FirmContact { Id = 3, FirmAddressId = 2, ContactType = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::FirmContact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::FirmContact>>()) == Inquire(new Erm::FirmContact { Id = 1, FirmAddressId = 2, ContactType = 1 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::FirmContact { Id = 1, FirmAddressId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::FirmContact>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactDeleted()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::FirmContact>>()) == Inquire(new Erm::FirmContact { Id = 3, FirmAddressId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 })
                   .Has(new Facts::FirmContact { Id = 3, FirmAddressId = 2 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::FirmContact>(3))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::LegalPerson>>()) == Inquire(new Erm::LegalPerson { Id = 1, ClientId = 1 }));

            FactsDb.Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::LegalPerson>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::LegalPerson>>()) == Inquire(new Erm::LegalPerson { Id = 1, ClientId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::LegalPerson>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::LegalPerson>(4))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderCreated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Order>>()) == Inquire(new Erm::Order { Id = 2, FirmId = 1, WorkflowStepId = 4 }));

            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Order>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderUpdated()
        {
            var source = Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Order>>()) == Inquire(new Erm::Order { Id = 1, FirmId = 2, WorkflowStepId = 4 }));

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::Order { Id = 1, FirmId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Order>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderDeleted()
        {
            var source = Mock.Of<IQuery>();

            FactsDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Order { Id = 2, FirmId = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::Order>(2))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(1)));
        }

        [Test]
        public void ShouldEnqueueOperationsInOrderForFirmIfFirmAddressCreated()
        {
            var source = Mock.Of<IQuery>(query =>
                query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(new Erm::Firm { Id = 2 }) &&
                query.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()) == Inquire(new Erm::FirmAddress { Id = 1, FirmId = 1 }, new Erm::FirmAddress { Id = 2, FirmId = 2 }));

            FactsDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(source, FactsQuery, FactsDb)
                          .Transform(Fact.Operation<Facts::FirmAddress>(1), Fact.Operation<Facts::Firm>(2), Fact.Operation<Facts::FirmAddress>(2))
                          .Verify(Inquire(Aggregate.Initialize<CI::Firm>(2), Aggregate.Recalculate<CI::Firm>(1), Aggregate.Recalculate<CI::Firm>(2)));
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
                yield return CaseToVerifyElementInsertion<Erm::Category, Facts::Category>(new Erm::Category { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::CategoryFirmAddress, Facts::CategoryFirmAddress>(new Erm::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::CategoryGroup, Facts::CategoryGroup>(new Erm::CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::CategoryOrganizationUnit, Facts::CategoryOrganizationUnit>(new Erm::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::Client, Facts::Client>(new Erm::Client { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::Contact, Facts::Contact>(new Erm::Contact { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::Firm, Facts::Firm>(new Erm::Firm { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::FirmAddress, Facts::FirmAddress>(new Erm::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::FirmContact, Facts::FirmContact>(new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = notnull });
                yield return CaseToVerifyElementInsertion<Erm::LegalPerson, Facts::LegalPerson>(new Erm::LegalPerson { Id = 1, ClientId = notnull });
                yield return CaseToVerifyElementInsertion<Erm::Order, Facts::Order>(new Erm::Order { Id = 1, WorkflowStepId = 4 });
                yield return CaseToVerifyElementInsertion<Erm::Project, Facts::Project>(new Erm::Project { Id = 1, OrganizationUnitId = notnull });
                yield return CaseToVerifyElementInsertion<Erm::Territory, Facts::Territory>(new Erm::Territory { Id = 1 });

                // update
                yield return CaseToVerifyElementUpdate(new Erm::Account { Id = 1 }, new Facts::Account { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::BranchOfficeOrganizationUnit { Id = 1 }, new Facts::BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Category { Id = 1 }, new Facts::Category { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::CategoryFirmAddress { Id = 1 }, new Facts::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::CategoryGroup { Id = 1 }, new Facts::CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::CategoryOrganizationUnit { Id = 1 }, new Facts::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Client { Id = 1 }, new Facts::Client { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Contact { Id = 1 }, new Facts::Contact { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Firm { Id = 1 }, new Facts::Firm { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::FirmAddress { Id = 1 }, new Facts::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = notnull }, new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = notnull });
                yield return CaseToVerifyElementUpdate(new Erm::LegalPerson { Id = 1, ClientId = notnull }, new Facts::LegalPerson { Id = 1, ClientId = notnull });
                yield return CaseToVerifyElementUpdate(new Erm::Order { Id = 1, WorkflowStepId = 4 }, new Facts::Order { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Project { Id = 1, OrganizationUnitId = notnull }, new Facts::Project { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Territory { Id = 1 }, new Facts::Territory { Id = 1 });

                // delete
                yield return CaseToVerifyElementDeletion(new Facts::Account { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Category { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Client { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Contact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Firm { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::FirmContact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::LegalPerson { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Order { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Project { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Territory { Id = 1 });
            }
        }

        private TestCaseData CaseToVerifyElementInsertion<TErmElement, TFactElement>(TErmElement source) where TErmElement : IIdentifiable, new()
            where TFactElement : class, IIdentifiable, new()
        {
            return Case(() => VerifyElementInsertion<TErmElement, TFactElement>(source))
                .SetName(string.Format("Should process and insert {0} element.", typeof(TFactElement).Name));
        }

        private TestCaseData CaseToVerifyElementUpdate<TErmElement, TFactElement>(TErmElement source, TFactElement target) where TErmElement : IIdentifiable, new()
            where TFactElement : class, IIdentifiable, new()
        {
            return Case(() => VerifyElementUpdate(source, target))
                .SetName(string.Format("Should process and update {0} element.", typeof(TFactElement).Name));
        }

        private TestCaseData CaseToVerifyElementDeletion<TFactElement>(TFactElement target) where TFactElement : class, IIdentifiable, new()
        {
            return Case(() => VerifyElementDeletion(target))
                .SetName(string.Format("Should process and delete {0} element.", typeof(TFactElement).Name));
        }

        private void VerifyElementInsertion<TErmElement, TFactElement>(TErmElement source) where TErmElement : IIdentifiable, new()
            where TFactElement : class, IIdentifiable, new()
        {
            var entityId = source.Id;
            ErmDb.Has(source);

            Transformation.Create(ErmQuery, FactsQuery)
                          .Transform(Fact.Operation<TFactElement>(entityId))
                          .Verify<TFactElement>(
                              x => x.AddRange(It.Is(Predicate.ByIds<TFactElement>(new[] { entityId }))),
                              Times.Once,
                              string.Format("The {0} element was not inserted.", typeof(TFactElement).Name));
        }

        private void VerifyElementUpdate<TErmElement, TFactElement>(TErmElement source, TFactElement target) where TErmElement : IIdentifiable, new()
            where TFactElement : class, IIdentifiable, new()
        {
            ErmDb.Has(source);
            FactsDb.Has(target);

            Transformation.Create(ErmQuery, FactsQuery)
                          .Transform(Fact.Operation<TFactElement>(target.Id))
                          .Verify<TFactElement>(
                              x => x.Update(It.Is(Predicate.ById<TFactElement>(target.Id))),
                              Times.Once,
                              string.Format("The {0} element was not updated.", typeof(TFactElement).Name));
        }

        private void VerifyElementDeletion<TFactElement>(TFactElement target) where TFactElement : class, IIdentifiable, new()
        {
            FactsDb.Has(target);

            Transformation.Create(ErmQuery, FactsQuery)
                          .Transform(Fact.Operation<TFactElement>(target.Id))
                          .Verify<TFactElement>(
                              x => x.DeleteRange(It.Is(Predicate.ByIds<TFactElement>(new[] { target.Id }))),
                              Times.Once,
                              string.Format("The {0} element was not deleted.", typeof(TFactElement).Name));
        }

        #region Transformation

        private class Transformation
        {
            private static IRepository _repositoryToVerify;
            
            private readonly ErmFactsTransformation _transformation;
            private readonly List<IOperation> _operations = new List<IOperation>();

            private Transformation(
                IQuery source,
                IQuery target,
                IFactChangesApplierFactory factChangesApplierFactory)
            {
                _transformation = new ErmFactsTransformation(source, target, factChangesApplierFactory);
            }

            private Transformation(IQuery source, IQuery target)
                : this(source, target, new VerifiableFactChangesApplierFactory(OnRepositoryCreated))
            {
            }

            public static Transformation Create(IQuery source, IQuery target)
            {
                return new Transformation(source, target);
            }

            public static Transformation Create(IQuery source, IQuery target, DataConnection dataConnection)
            {
                return new Transformation(
                    source,
                    target,
                    new StubFactChangesApplierFactory(dataConnection.Connection, dataConnection));
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