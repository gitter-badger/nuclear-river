using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture, SetCulture("")]
    internal partial class ErmMapToFactsSpecsTests : TransformationFixtureBase
    {
        private static readonly DateTimeOffset Date = new DateTimeOffset(2015, 04, 03, 12, 30, 00, new TimeSpan());

        [Test]
        public void ShouldTransformAccount()
        {
            var query = new MemoryMockQuery(
                new Erm::Account { Id = 1, Balance = 123.45m, LegalPersonId = 2 });

            Transformation.Create(query)
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.Accounts.Map(q).ById(1), Inquire(new Facts::Account { Id = 1, Balance = 123.45m, LegalPersonId = 2 }));
        }

        [Test]
        public void ShouldTransformBranchOfficeOrganizationUnit()
        {
            var query = new MemoryMockQuery(
                new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });
            
            Transformation.Create(query)
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.BranchOfficeOrganizationUnits.Map(q).ById(1),
                                           Inquire(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 }));
        }

        [Test]
        public void ShouldTransformCategory()
        {
            var query = new MemoryMockQuery(
                new Erm::Category { Id = 1, Level = 2, ParentId = 3 });

            Transformation.Create(query)
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.Categories.Map(q).ById(1), Inquire(new Facts::Category { Id = 1, Level = 2, ParentId = 3 }));
        }

        [Test]
        public void ShouldTransformCategoryFirmAddress()
        {
            var query = new MemoryMockQuery(
                new Erm::CategoryFirmAddress { Id = 1, CategoryId = 2, FirmAddressId = 3 });

            Transformation.Create(query)
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.CategoryFirmAddresses.Map(q).ById(1), Inquire(new Facts::CategoryFirmAddress { Id = 1, CategoryId = 2, FirmAddressId = 3 }));
        }

        [Test]
        public void ShouldTransformCategoryGroup()
        {
            var query = new MemoryMockQuery(
                new Erm::CategoryGroup { Id = 1, Name = "name", Rate = 1 });
            
            Transformation.Create(query)
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.CategoryGroups.Map(q).ById(1), Inquire(new Facts::CategoryGroup { Id = 1, Name = "name", Rate = 1 }));
        }

        [Test]
        public void ShouldTransformCategoryOrganizationUnit()
        {
            var query = new MemoryMockQuery(
                new Erm::CategoryOrganizationUnit { Id = 1, CategoryId = 2, CategoryGroupId = 3, OrganizationUnitId = 4 });

            Transformation.Create(query)
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.CategoryOrganizationUnits.Map(q).ById(1), Inquire(new Facts::CategoryOrganizationUnit { Id = 1, CategoryId = 2, CategoryGroupId = 3, OrganizationUnitId = 4 }));
        }

        [Test]
        public void ShouldTransformClient()
        {
            var query = new MemoryMockQuery(
                new Erm::Client { Id = 1, Name = "client", LastDisqualifyTime = Date },
                new Erm::Client { Id = 2, MainPhoneNumber = "phone" },
                new Erm::Client { Id = 3, AdditionalPhoneNumber1 = "phone" },
                new Erm::Client { Id = 4, AdditionalPhoneNumber2 = "phone" },
                new Erm::Client { Id = 5, Website = "site" });

            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Clients.Map(x).ById(1), Inquire(new Facts::Client { Id = 1, Name = "client", LastDisqualifiedOn = Date }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Clients.Map(x).ById(2), Inquire(new Facts::Client { Id = 2, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Clients.Map(x).ById(3), Inquire(new Facts::Client { Id = 3, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Clients.Map(x).ById(4), Inquire(new Facts::Client { Id = 4, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Clients.Map(x).ById(5), Inquire(new Facts::Client { Id = 5, HasWebsite = true }));
        }

        [Test]
        public void ShouldTransformContact()
        {
            var query = new MemoryMockQuery(
                new Erm::Contact { Id = 1, IsFired = true, ClientId = 2 },
                new Erm::Contact { Id = 2, Role = 200000 },
                new Erm::Contact { Id = 3, Role = 200001 },
                new Erm::Contact { Id = 4, Role = 200002 },
                new Erm::Contact { Id = 5, MainPhoneNumber = "phone" },
                new Erm::Contact { Id = 6, MobilePhoneNumber = "phone" },
                new Erm::Contact { Id = 7, HomePhoneNumber = "phone" },
                new Erm::Contact { Id = 8, AdditionalPhoneNumber = "phone" },
                new Erm::Contact { Id = 9, Website = "site" });

            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts.Map(x).ById(1), Inquire(new Facts::Contact { Id = 1, IsFired = true, ClientId = 2 }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts.Map(x).ById(2), Inquire(new Facts::Contact { Id = 2, Role = 1 }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts.Map(x).ById(3), Inquire(new Facts::Contact { Id = 3, Role = 2 }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts.Map(x).ById(4), Inquire(new Facts::Contact { Id = 4, Role = 3 }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts.Map(x).ById(5), Inquire(new Facts::Contact { Id = 5, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts.Map(x).ById(6), Inquire(new Facts::Contact { Id = 6, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts.Map(x).ById(7), Inquire(new Facts::Contact { Id = 7, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts.Map(x).ById(8), Inquire(new Facts::Contact { Id = 8, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts.Map(x).ById(9), Inquire(new Facts::Contact { Id = 9, HasWebsite = true }));
        }

        [Test]
        public void ShouldTransformFirm()
        {
            var query = new MemoryMockQuery(
                new Erm::Firm { Id = 1, Name = "firm", CreatedOn = Date, LastDisqualifyTime = Date.AddDays(1), ClientId = 2, OrganizationUnitId = 3, TerritoryId = 4, OwnerId = 5 });

            Transformation.Create(query)
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.Firms.Map(x).ById(1), Inquire(new Facts::Firm { Id = 1, Name = "firm", CreatedOn = Date, LastDisqualifiedOn = Date.AddDays(1), ClientId = 2, OrganizationUnitId = 3, TerritoryId = 4, OwnerId = 5 }));
        }

        [Test]
        public void ShouldTransformFirmAddress()
        {
            var query = new MemoryMockQuery(
                new Erm::FirmAddress { Id = 1, FirmId = 2 });

            Transformation.Create(query)
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.FirmAddresses.Map(x).ById(1), Inquire(new Facts::FirmAddress { Id = 1, FirmId = 2 }));
        }

        [Test]
        public void ShouldTransformFirmContact()
        {
            const long NotNull = 123;

            var query = new MemoryMockQuery(
                new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull },
                new Erm::FirmContact { Id = 2, ContactType = 1, FirmAddressId = null },
                new Erm::FirmContact { Id = 3, ContactType = 2, FirmAddressId = NotNull },
                new Erm::FirmContact { Id = 4, ContactType = 4, FirmAddressId = NotNull });

            Transformation.Create(query)
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.FirmContacts.Map(x).ById(1), Inquire(new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = NotNull }))
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.FirmContacts.Map(x).ById(2), Inquire<Facts::FirmContact>())
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.FirmContacts.Map(x).ById(3), Inquire<Facts::FirmContact>())
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.FirmContacts.Map(x).ById(4), Inquire(new Facts::FirmContact { Id = 4, HasWebsite = true, FirmAddressId = NotNull }));
        }

        [Test]
        public void ShouldTransformLegalPerson()
        {
            var query = new MemoryMockQuery(
                new Erm::LegalPerson { Id = 1, ClientId = 2 },
                new Erm::LegalPerson { Id = 2, ClientId = null });

            Transformation.Create(query)
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.LegalPersons.Map(x).ById(1), Inquire(new Facts::LegalPerson { Id = 1, ClientId = 2 }))
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.LegalPersons.Map(x).ById(2), Inquire<Facts::LegalPerson>());
        }

        [Test]
        public void ShouldTransformOrder()
        {
            var query = new MemoryMockQuery(
                new Erm::Order { Id = 1, EndDistributionDateFact = Date, WorkflowStepId = 1, FirmId = 2 },
                new Erm::Order { Id = 2, EndDistributionDateFact = Date, WorkflowStepId = 4 /* on termination*/, FirmId = 2 });

            Transformation.Create(query)
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.Orders.Map(x).ById(1), Inquire<Facts::Order>())
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.Orders.Map(x).ById(2), Inquire(new Facts::Order { Id = 2, EndDistributionDateFact = Date, FirmId = 2 }));
        }

        [Test]
        public void ShouldTransformProject()
        {
            var query = new MemoryMockQuery(
                new Erm::Project { Id = 1, Name = "name", OrganizationUnitId = 2 });

            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Projects.Map(x).ById(1), Inquire(new Facts::Project { Id = 1, Name = "name", OrganizationUnitId = 2 }));
        }

        [Test]
        public void ShouldTransformTerritory()
        {
            var query = new MemoryMockQuery(
                new Erm::Territory { Id = 1, Name = "name", OrganizationUnitId = 2 });

            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Territories.Map(x).ById(1), Inquire(new Facts::Territory { Id = 1, Name = "name", OrganizationUnitId = 2 }));
        }

        #region Transformation

        private class Transformation
        {
            private readonly IQuery _query;

            private Transformation(IQuery query)
            {
                _query = query;
            }

            public static Transformation Create(IQuery query)
            {
                return new Transformation(query);
            }

            public Transformation VerifyTransform<T>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, string message = null)
            {
                VerifyTransform(reader, expected, x => x, message);
                return this;
            }

            public Transformation VerifyTransform<T, TProjection>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                // TODO: convert to a custom NUnit constraint, at least for fail logging
                Assert.That(reader(_query).ToArray, Is.EqualTo(expected.ToArray()).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }

        #endregion
    }
}