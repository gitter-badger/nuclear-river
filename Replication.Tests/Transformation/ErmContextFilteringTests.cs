using System;
using System.Collections.Generic;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;
using NuClear.AdvancedSearch.Replication.Tests.Data;

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
            ErmDb.Has(new Account { Id = 1, Balance = 123.45m, LegalPersonId = 1 })
                         .Has(new Account { Id = 2, IsActive = false, IsDeleted = false })
                         .Has(new Account { Id = 3, IsActive = false, IsDeleted = true })
                         .Has(new Account { Id = 4, IsActive = true, IsDeleted = true });

            Reader.Create(ErmDb)
                .VerifyRead(x => x.Accounts.ById(1), Inquire(new Account { Id = 1, Balance = 123.45m, LegalPersonId = 1 }))
                .VerifyRead(x => x.Accounts.ById(2), Inquire<Account>())
                .VerifyRead(x => x.Accounts.ById(3), Inquire<Account>())
                .VerifyRead(x => x.Accounts.ById(4), Inquire<Account>());
        }

        [Test]
        public void ShouldReadBranchOfficeOrganizationUnit()
        {
            ErmDb.Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 123 })
                         .Has(new BranchOfficeOrganizationUnit { Id = 2, IsActive = false, IsDeleted = false })
                         .Has(new BranchOfficeOrganizationUnit { Id = 3, IsActive = false, IsDeleted = true })
                         .Has(new BranchOfficeOrganizationUnit { Id = 4, IsActive = true, IsDeleted = true });

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.BranchOfficeOrganizationUnits.ById(1), Inquire(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 123 }))
                  .VerifyRead(x => x.BranchOfficeOrganizationUnits.ById(2), Inquire<BranchOfficeOrganizationUnit>())
                  .VerifyRead(x => x.BranchOfficeOrganizationUnits.ById(3), Inquire<BranchOfficeOrganizationUnit>())
                  .VerifyRead(x => x.BranchOfficeOrganizationUnits.ById(4), Inquire<BranchOfficeOrganizationUnit>());
        }

        [Test]
        public void ShouldReadCategories()
        {
            ErmDb.Has(new Category { Id = 1, Level = 1, ParentId = 123 })
                 .Has(new Category { Id = 2, IsActive = false, IsDeleted = false })
                 .Has(new Category { Id = 3, IsActive = false, IsDeleted = true })
                 .Has(new Category { Id = 4, IsActive = true, IsDeleted = true });

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.Categories.ById(1), Inquire(new Category { Id = 1, Level = 1, ParentId = 123 }))
                  .VerifyRead(x => x.Categories.ById(2), Inquire<Category>())
                  .VerifyRead(x => x.Categories.ById(3), Inquire<Category>())
                  .VerifyRead(x => x.Categories.ById(4), Inquire<Category>());
        }

        [Test]
        public void ShouldReadCategoryFirmAddress()
        {
            ErmDb.Has(new CategoryFirmAddress { Id = 1, CategoryId = 123, FirmAddressId = 456 })
                         .Has(new CategoryFirmAddress { Id = 2, IsActive = false, IsDeleted = false })
                         .Has(new CategoryFirmAddress { Id = 3, IsActive = false, IsDeleted = true })
                         .Has(new CategoryFirmAddress { Id = 4, IsActive = true, IsDeleted = true });

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.CategoryFirmAddresses.ById(1), Inquire(new CategoryFirmAddress { Id = 1, CategoryId = 123, FirmAddressId = 456 }))
                  .VerifyRead(x => x.CategoryFirmAddresses.ById(2), Inquire<CategoryFirmAddress>())
                  .VerifyRead(x => x.CategoryFirmAddresses.ById(3), Inquire<CategoryFirmAddress>())
                  .VerifyRead(x => x.CategoryFirmAddresses.ById(4), Inquire<CategoryFirmAddress>());
        }

        [Test]
        public void ShouldReadCategoryOrganizationUnit()
        {
            ErmDb.Has(new CategoryOrganizationUnit { Id = 1, CategoryId = 123, CategoryGroupId = 456, OrganizationUnitId = 789 })
                         .Has(new CategoryOrganizationUnit { Id = 2, IsActive = false, IsDeleted = false })
                         .Has(new CategoryOrganizationUnit { Id = 3, IsActive = false, IsDeleted = true })
                         .Has(new CategoryOrganizationUnit { Id = 4, IsActive = true, IsDeleted = true });

            Reader.Create(ErmDb)
                  .VerifyRead(
                      x => x.CategoryOrganizationUnits.ById(1),
                      Inquire(new CategoryOrganizationUnit { Id = 1, CategoryId = 123, CategoryGroupId = 456, OrganizationUnitId = 789 }))
                  .VerifyRead(x => x.CategoryOrganizationUnits.ById(2), Inquire<CategoryOrganizationUnit>())
                  .VerifyRead(x => x.CategoryOrganizationUnits.ById(3), Inquire<CategoryOrganizationUnit>())
                  .VerifyRead(x => x.CategoryOrganizationUnits.ById(4), Inquire<CategoryOrganizationUnit>());
        }

        [Test]
        public void ShouldReadClient()
        {
            ErmDb.Has(
                new Client
                {
                    Id = 1,
                    Name = "client",
                    AdditionalPhoneNumber1 = "phone1",
                    AdditionalPhoneNumber2 = "phone2",
                    MainPhoneNumber = "phone3",
                    Website = "site",
                    LastDisqualifyTime = Date
                })
                         .Has(new Client { Id = 2, IsActive = false, IsDeleted = false })
                         .Has(new Client { Id = 3, IsActive = false, IsDeleted = true })
                         .Has(new Client { Id = 4, IsActive = true, IsDeleted = true });

            Reader.Create(ErmDb)
                  .VerifyRead(
                      x => x.Clients.ById(1),
                      Inquire(
                          new Client
                          {
                              Id = 1,
                              Name = "client",
                              AdditionalPhoneNumber1 = "phone1",
                              AdditionalPhoneNumber2 = "phone2",
                              MainPhoneNumber = "phone3",
                              Website = "site",
                              LastDisqualifyTime = Date
                          }))
                  .VerifyRead(x => x.Clients.ById(2), Inquire<Client>())
                  .VerifyRead(x => x.Clients.ById(3), Inquire<Client>())
                  .VerifyRead(x => x.Clients.ById(4), Inquire<Client>());
        }

        [Test]
        public void ShouldReadContact()
        {
            ErmDb.Has(
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
                })
                         .Has(new Contact { Id = 2, IsActive = false, IsDeleted = false })
                         .Has(new Contact { Id = 3, IsActive = false, IsDeleted = true })
                         .Has(new Contact { Id = 4, IsActive = true, IsDeleted = true });

            Reader.Create(ErmDb)
                  .VerifyRead(
                      x => x.Contacts.ById(1),
                      Inquire(
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
                          }))
                  .VerifyRead(x => x.Contacts.ById(2), Inquire<Contact>())
                  .VerifyRead(x => x.Contacts.ById(3), Inquire<Contact>())
                  .VerifyRead(x => x.Contacts.ById(4), Inquire<Contact>());
        }

        [Test]
        public void ShouldReadFirm()
        {
            ErmDb.Has(new Firm { Id = 1, Name = "firm", CreatedOn = Date, LastDisqualifyTime = Date.AddHours(1), ClientId = 1, OrganizationUnitId = 2, TerritoryId = 3 })
                         .Has(new Firm { Id = 2, IsActive = false })
                         .Has(new Firm { Id = 3, IsDeleted = true })
                         .Has(new Firm { Id = 4, IsActive = false, IsDeleted = true })
                         .Has(new Firm { Id = 5, ClosedForAscertainment = true });

            Reader.Create(ErmDb)
                  .VerifyRead(
                      x => x.Firms.ById(1),
                      Inquire(new Firm { Id = 1, Name = "firm", CreatedOn = Date, LastDisqualifyTime = Date.AddHours(1), ClientId = 1, OrganizationUnitId = 2, TerritoryId = 3 }))
                  .VerifyRead(x => x.Firms.ById(2), Inquire<Firm>())
                  .VerifyRead(x => x.Firms.ById(3), Inquire<Firm>())
                  .VerifyRead(x => x.Firms.ById(4), Inquire<Firm>())
                  .VerifyRead(x => x.Firms.ById(5), Inquire<Firm>());
        }

        [Test]
        public void ShouldReadFirmAddress()
        {
            ErmDb.Has(new FirmAddress { Id = 1, FirmId = 1 })
                         .Has(new FirmAddress { Id = 2, IsActive = false })
                         .Has(new FirmAddress { Id = 3, IsDeleted = true })
                         .Has(new FirmAddress { Id = 4, IsActive = false, IsDeleted = true })
                         .Has(new FirmAddress { Id = 5, ClosedForAscertainment = true });

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.FirmAddresses.ById(1), Inquire(new FirmAddress { Id = 1, FirmId = 1 }))
                  .VerifyRead(x => x.FirmAddresses.ById(2), Inquire<FirmAddress>())
                  .VerifyRead(x => x.FirmAddresses.ById(3), Inquire<FirmAddress>())
                  .VerifyRead(x => x.FirmAddresses.ById(4), Inquire<FirmAddress>())
                  .VerifyRead(x => x.FirmAddresses.ById(5), Inquire<FirmAddress>());
        }

        [Test]
        public void ShouldReadFirmContact()
        {
            ErmDb.Has(new FirmContact { Id = 1, ContactType = 1, FirmAddressId = 1 })
                         .Has(new FirmContact { Id = 2, ContactType = 2, FirmAddressId = 1 })
                         .Has(new FirmContact { Id = 3, ContactType = 4, FirmAddressId = 2 });

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.FirmContacts.ById(1), Inquire(new FirmContact { Id = 1, ContactType = 1, FirmAddressId = 1 }))
                  .VerifyRead(x => x.FirmContacts.ById(2), Inquire(new FirmContact { Id = 2, ContactType = 2, FirmAddressId = 1 }))
                  .VerifyRead(x => x.FirmContacts.ById(3), Inquire(new FirmContact { Id = 3, ContactType = 4, FirmAddressId = 2 }));
        }

        [Test]
        public void ShouldReadLegalPerson()
        {
            ErmDb.Has(new LegalPerson { Id = 1, ClientId = 1 })
                         .Has(new LegalPerson { Id = 2, IsActive = false })
                         .Has(new LegalPerson { Id = 3, IsDeleted = true })
                         .Has(new LegalPerson { Id = 4, IsActive = false, IsDeleted = true });

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.LegalPersons.ById(1), Inquire(new LegalPerson { Id = 1, ClientId = 1 }))
                  .VerifyRead(x => x.LegalPersons.ById(2), Inquire<LegalPerson>())
                  .VerifyRead(x => x.LegalPersons.ById(3), Inquire<LegalPerson>())
                  .VerifyRead(x => x.LegalPersons.ById(4), Inquire<LegalPerson>());
        }

        [Test]
        public void ShouldReadOrder()
        {
            ErmDb.Has(new Order { Id = 1, EndDistributionDateFact = Date, WorkflowStepId = 4, FirmId = 3 })
                         .Has(new Order { Id = 2, IsActive = false })
                         .Has(new Order { Id = 3, IsDeleted = true })
                         .Has(new Order { Id = 4, IsActive = false, IsDeleted = true })
                         .Has(new Order { Id = 5, WorkflowStepId = 1 });

            Reader.Create(ErmDb)
                  .VerifyRead(x => x.Orders.ById(1), Inquire(new Order { Id = 1, EndDistributionDateFact = Date, WorkflowStepId = 4, FirmId = 3 }))
                  .VerifyRead(x => x.Orders.ById(2), Inquire<Order>())
                  .VerifyRead(x => x.Orders.ById(3), Inquire<Order>())
                  .VerifyRead(x => x.Orders.ById(4), Inquire<Order>())
                  .VerifyRead(x => x.Orders.ById(5), Inquire(new Order { Id = 5, WorkflowStepId = 1 }));
        }

        #region Reader

        private class Reader
        {
            private readonly IErmContext _transformation;

            private Reader(IDataContext source)
            {
                _transformation = new ErmContext(source);
            }

            public static Reader Create(IDataContext source)
            {
                return new Reader(source);
            }

            public Reader VerifyRead<T>(Func<IErmContext, IEnumerable<T>> reader, IEnumerable<T> expected, string message = null)
            {
                VerifyRead(reader, expected, x => x, message);
                return this;
            }

            public Reader VerifyRead<T, TProjection>(Func<IErmContext, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                Assert.That(reader(_transformation), Is.EqualTo(expected).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }

        #endregion
    }
}