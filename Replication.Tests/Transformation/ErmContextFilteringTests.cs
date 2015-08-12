using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;
using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.AdvancedSearch.Replication.Tests.Data;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    [TestFixture]
    internal class ErmContextFilteringTests : BaseTransformationFixture
    {
        private static readonly DateTimeOffset Date = new DateTimeOffset(2015, 04, 03, 12, 30, 00, new TimeSpan());

        [Test]
        public void ShouldReadAccount()
        {
            var data = new[]
                       {
                           new Account { Id = 1, Balance = 123.45m, LegalPersonId = 1 },
                           new Account { Id = 2, IsActive = false, IsDeleted = false },
                           new Account { Id = 3, IsActive = false, IsDeleted = true },
                           new Account { Id = 4, IsActive = true, IsDeleted = true },
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.Accounts(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.Accounts(new[] { 2L })), Inquire<Account>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Accounts(new[] { 3L })), Inquire<Account>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Accounts(new[] { 4L })), Inquire<Account>());
        }

        [Test]
        public void ShouldReadBranchOfficeOrganizationUnit()
        {
            var data = new[]
                       {
                           new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 123 },
                           new BranchOfficeOrganizationUnit { Id = 2, IsActive = false, IsDeleted = false },
                           new BranchOfficeOrganizationUnit { Id = 3, IsActive = false, IsDeleted = true },
                           new BranchOfficeOrganizationUnit { Id = 4, IsActive = true, IsDeleted = true }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.BranchOfficeOrganizationUnits(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.BranchOfficeOrganizationUnits(new[] { 2L })), Inquire<BranchOfficeOrganizationUnit>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.BranchOfficeOrganizationUnits(new[] { 3L })), Inquire<BranchOfficeOrganizationUnit>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.BranchOfficeOrganizationUnits(new[] { 4L })), Inquire<BranchOfficeOrganizationUnit>());
        }

        [Test]
        public void ShouldReadCategory()
        {
            var data = new[]
                       {
                           new Category { Id = 1, Level = 1, ParentId = 123 },
                           new Category { Id = 2, IsActive = false, IsDeleted = false },
                           new Category { Id = 3, IsActive = false, IsDeleted = true },
                           new Category { Id = 4, IsActive = true, IsDeleted = true }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.Categories(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.Categories(new[] { 2L })), Inquire<Category>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Categories(new[] { 3L })), Inquire<Category>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Categories(new[] { 4L })), Inquire<Category>());
        }

        [Test]
        public void ShouldReadCategoryFirmAddress()
        {
            var data = new[]
                       {
                           new CategoryFirmAddress { Id = 1, CategoryId = 123, FirmAddressId = 456 },
                           new CategoryFirmAddress { Id = 2, IsActive = false, IsDeleted = false },
                           new CategoryFirmAddress { Id = 3, IsActive = false, IsDeleted = true },
                           new CategoryFirmAddress { Id = 4, IsActive = true, IsDeleted = true }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryFirmAddresses(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryFirmAddresses(new[] { 2L })), Inquire<CategoryFirmAddress>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryFirmAddresses(new[] { 3L })), Inquire<CategoryFirmAddress>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryFirmAddresses(new[] { 4L })), Inquire<CategoryFirmAddress>());
        }

        [Test]
        public void ShouldReadCategoryGroup()
        {
            var data = new[]
                       {
                           new CategoryGroup { Id = 1, Name = "name", Rate = 1 },
                           new CategoryGroup { Id = 2, IsActive = false, IsDeleted = false },
                           new CategoryGroup { Id = 3, IsActive = false, IsDeleted = true },
                           new CategoryGroup { Id = 4, IsActive = true, IsDeleted = true }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryGroups(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryGroups(new[] { 2L })), Inquire<CategoryGroup>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryGroups(new[] { 3L })), Inquire<CategoryGroup>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryGroups(new[] { 4L })), Inquire<CategoryGroup>());
        }

        [Test]
        public void ShouldReadCategoryOrganizationUnit()
        {
            var data = new[]
                       {
                           new CategoryOrganizationUnit { Id = 1, CategoryId = 123, CategoryGroupId = 456, OrganizationUnitId = 789 },
                           new CategoryOrganizationUnit { Id = 2, IsActive = false, IsDeleted = false },
                           new CategoryOrganizationUnit { Id = 3, IsActive = false, IsDeleted = true },
                           new CategoryOrganizationUnit { Id = 4, IsActive = true, IsDeleted = true }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryOrganizationUnits(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryOrganizationUnits(new[] { 2L })), Inquire<CategoryOrganizationUnit>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryOrganizationUnits(new[] { 3L })), Inquire<CategoryOrganizationUnit>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.CategoryOrganizationUnits(new[] { 4L })), Inquire<CategoryOrganizationUnit>());
        }

        [Test]
        public void ShouldReadClient()
        {
            var data = new[]
                       {
                           new Client
                           {
                               Id = 1,
                               Name = "client",
                               AdditionalPhoneNumber1 = "phone1",
                               AdditionalPhoneNumber2 = "phone2",
                               MainPhoneNumber = "phone3",
                               Website = "site",
                               LastDisqualifyTime = Date
                           },
                           new Client { Id = 2, IsActive = false, IsDeleted = false },
                           new Client { Id = 3, IsActive = false, IsDeleted = true },
                           new Client { Id = 4, IsActive = true, IsDeleted = true }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.Clients(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.Clients(new[] { 2L })), Inquire<Client>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Clients(new[] { 3L })), Inquire<Client>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Clients(new[] { 4L })), Inquire<Client>());
        }

        [Test]
        public void ShouldReadContact()
        {
            var data = new[]
                       {
                           new Contact
                           {
                               Id = 1,
                               Role = 1,
                               IsFired = true,
                               MainPhoneNumber = "phone1",
                               HomePhoneNumber = "phone2",
                               MobilePhoneNumber = "phone3",
                               AdditionalPhoneNumber = "phone4",
                               Website = "site",
                               ClientId = 1
                           },
                           new Contact { Id = 2, IsActive = false, IsDeleted = false },
                           new Contact { Id = 3, IsActive = false, IsDeleted = true },
                           new Contact { Id = 4, IsActive = true, IsDeleted = true }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.Contacts(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.Contacts(new[] { 2L })), Inquire<Contact>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Contacts(new[] { 3L })), Inquire<Contact>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Contacts(new[] { 4L })), Inquire<Contact>());
        }

        [Test]
        public void ShouldReadFirm()
        {
            var data = new[]
                       {
                           new Firm { Id = 1, Name = "firm", CreatedOn = Date, LastDisqualifyTime = Date.AddHours(1), ClientId = 1, OrganizationUnitId = 2, TerritoryId = 3 },
                           new Firm { Id = 2, IsActive = false },
                           new Firm { Id = 3, IsDeleted = true },
                           new Firm { Id = 4, IsActive = false, IsDeleted = true },
                           new Firm { Id = 5, ClosedForAscertainment = true }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3])
                 .Has(data[4]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.Firms(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.Firms(new[] { 2L })), Inquire<Firm>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Firms(new[] { 3L })), Inquire<Firm>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Firms(new[] { 4L })), Inquire<Firm>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Firms(new[] { 5L })), Inquire<Firm>());
        }

        [Test]
        public void ShouldReadFirmAddress()
        {
            var data = new[]
                       {
                           new FirmAddress { Id = 1, FirmId = 1 },
                           new FirmAddress { Id = 2, IsActive = false },
                           new FirmAddress { Id = 3, IsDeleted = true },
                           new FirmAddress { Id = 4, IsActive = false, IsDeleted = true },
                           new FirmAddress { Id = 5, ClosedForAscertainment = true }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3])
                 .Has(data[4]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.FirmAddresses(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.FirmAddresses(new[] { 2L })), Inquire<FirmAddress>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.FirmAddresses(new[] { 3L })), Inquire<FirmAddress>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.FirmAddresses(new[] { 4L })), Inquire<FirmAddress>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.FirmAddresses(new[] { 5L })), Inquire(data[4]));
        }

        [Test]
        public void ShouldReadFirmContact()
        {
            var data = new[]
                       {
                           new FirmContact { Id = 1, ContactType = 1, FirmAddressId = 1 },
                           new FirmContact { Id = 2, ContactType = 2, FirmAddressId = 1 },
                           new FirmContact { Id = 3, ContactType = 4, FirmAddressId = 2 }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.FirmContacts(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.FirmContacts(new[] { 2L })), Inquire(data[1]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.FirmContacts(new[] { 3L })), Inquire(data[2]));
        }

        [Test]
        public void ShouldReadLegalPerson()
        {
            var data = new[]
                       {
                           new LegalPerson { Id = 1, ClientId = 1 },
                           new LegalPerson { Id = 2, IsActive = false },
                           new LegalPerson { Id = 3, IsDeleted = true },
                           new LegalPerson { Id = 4, IsActive = false, IsDeleted = true }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.LegalPersons(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.LegalPersons(new[] { 2L })), Inquire<LegalPerson>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.LegalPersons(new[] { 3L })), Inquire<LegalPerson>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.LegalPersons(new[] { 4L })), Inquire<LegalPerson>());
        }

        [Test]
        public void ShouldReadOrder()
        {
            var data = new[]
                       {
                           new Order { Id = 1, EndDistributionDateFact = Date, WorkflowStepId = 4, FirmId = 3 },
                           new Order { Id = 2, IsActive = false },
                           new Order { Id = 3, IsDeleted = true },
                           new Order { Id = 4, IsActive = false, IsDeleted = true },
                           new Order { Id = 5, WorkflowStepId = 1 }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1])
                 .Has(data[2])
                 .Has(data[3])
                 .Has(data[4]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.Orders(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.Orders(new[] { 2L })), Inquire<Order>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Orders(new[] { 3L })), Inquire<Order>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Orders(new[] { 4L })), Inquire<Order>())
                  .VerifyRead(x => x.For(Specs.Erm.Find.Orders(new[] { 5L })), Inquire(data[4]));
        }

        [Test]
        public void ShouldReadProject()
        {
            var data = new[]
                       {
                           new Project { Id = 1, Name = "name", OrganizationUnitId = 2 },
                           new Project { Id = 2, IsActive = false }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.Projects(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.Projects(new[] { 2L })), Inquire<Project>());
        }

        [Test]
        public void ShouldReadTerritory()
        {
            var data = new[]
                       {
                           new Territory { Id = 1, Name = "name", OrganizationUnitId = 2 },
                           new Territory { Id = 2, IsActive = false }
                       };

            ErmDb.Has(data[0])
                 .Has(data[1]);

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.For(Specs.Erm.Find.Territories(new[] { 1L })), Inquire(data[0]))
                  .VerifyRead(x => x.For(Specs.Erm.Find.Territories(new[] { 2L })), Inquire<Territory>());
        }

        #region Reader

        private class Reader
        {
            private readonly IQuery _query;

            private Reader(DataConnection source)
            {
                _query = new Query(new StubReadableDomainContextProvider(source.Connection, source));
            }

            public static Reader Create(DataConnection source)
            {
                return new Reader(source);
            }

            public Reader VerifyRead<T>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, string message = null)
            {
                VerifyRead(reader, expected, x => x, message);
                return this;
            }

            private Reader VerifyRead<T, TProjection>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                Assert.That(reader(_query), Is.EqualTo(expected.ToArray()).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }

        #endregion
    }
}