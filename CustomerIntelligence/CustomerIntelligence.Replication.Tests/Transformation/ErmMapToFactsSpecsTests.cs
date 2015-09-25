using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Storage.Readings;

using NUnit.Framework;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;

namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture, SetCulture("")]
    internal partial class ErmMapToFactsSpecsTests : TransformationFixtureBase
    {
        private static readonly DateTimeOffset Date = new DateTimeOffset(2015, 04, 03, 12, 30, 00, new TimeSpan());

        [Test]
        public void ShouldTransformAccount()
        {
            SourceDb.Has(
                new Erm::Account { Id = 1, Balance = 123.45m, LegalPersonId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.Accounts.Map(q).ById(1), new Facts::Account { Id = 1, Balance = 123.45m, LegalPersonId = 2 });
        }

        [Test]
        public void ShouldTransformBranchOfficeOrganizationUnit()
        {
            SourceDb.Has(
                new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });
            
            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.BranchOfficeOrganizationUnits.Map(q).ById(1),
                                           new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });
        }

        [Test]
        public void ShouldTransformCategory()
        {
            SourceDb.Has(
                new Erm::Category { Id = 1, Level = 2, ParentId = 3 });

            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.Categories.Map(q).ById(1), new Facts::Category { Id = 1, Level = 2, ParentId = 3 });
        }

        [Test]
        public void ShouldTransformCategoryFirmAddress()
        {
            SourceDb.Has(
                new Erm::CategoryFirmAddress { Id = 1, CategoryId = 2, FirmAddressId = 3 });

            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.CategoryFirmAddresses.Map(q).ById(1), new Facts::CategoryFirmAddress { Id = 1, CategoryId = 2, FirmAddressId = 3 });
        }

        [Test]
        public void ShouldTransformCategoryGroup()
        {
            SourceDb.Has(
                new Erm::CategoryGroup { Id = 1, Name = "name", Rate = 1 });
            
            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.CategoryGroups.Map(q).ById(1), new Facts::CategoryGroup { Id = 1, Name = "name", Rate = 1 });
        }

        [Test]
        public void ShouldTransformCategoryOrganizationUnit()
        {
            SourceDb.Has(
                new Erm::CategoryOrganizationUnit { Id = 1, CategoryId = 2, CategoryGroupId = 3, OrganizationUnitId = 4 });

            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.CategoryOrganizationUnits.Map(q).ById(1),
                                           new Facts::CategoryOrganizationUnit { Id = 1, CategoryId = 2, CategoryGroupId = 3, OrganizationUnitId = 4 });
        }

        [Test]
        public void ShouldTransformClient()
        {
            SourceDb.Has(
                new Erm::Client { Id = 1, Name = "client", LastDisqualifyTime = Date },
                new Erm::Client { Id = 2, MainPhoneNumber = "phone" },
                new Erm::Client { Id = 3, AdditionalPhoneNumber1 = "phone" },
                new Erm::Client { Id = 4, AdditionalPhoneNumber2 = "phone" },
                new Erm::Client { Id = 5, Website = "site" });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Clients.Map(x).ById(1), new Facts::Client { Id = 1, Name = "client", LastDisqualifiedOn = Date })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Clients.Map(x).ById(2), new Facts::Client { Id = 2, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Clients.Map(x).ById(3), new Facts::Client { Id = 3, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Clients.Map(x).ById(4), new Facts::Client { Id = 4, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Clients.Map(x).ById(5), new Facts::Client { Id = 5, HasWebsite = true });
        }

        [Test]
        public void ShouldTransformContact()
        {
            SourceDb.Has(
                new Erm::Contact { Id = 1, ClientId = 2 },
                new Erm::Contact { Id = 2, Role = 200000 },
                new Erm::Contact { Id = 3, Role = 200001 },
                new Erm::Contact { Id = 4, Role = 200002 },
                new Erm::Contact { Id = 5, MainPhoneNumber = "phone" },
                new Erm::Contact { Id = 6, MobilePhoneNumber = "phone" },
                new Erm::Contact { Id = 7, HomePhoneNumber = "phone" },
                new Erm::Contact { Id = 8, AdditionalPhoneNumber = "phone" },
                new Erm::Contact { Id = 9, Website = "site" });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).ById(1), new Facts::Contact { Id = 1, ClientId = 2 })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).ById(2), new Facts::Contact { Id = 2, Role = 1 })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).ById(3), new Facts::Contact { Id = 3, Role = 2 })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).ById(4), new Facts::Contact { Id = 4, Role = 3 })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).ById(5), new Facts::Contact { Id = 5, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).ById(6), new Facts::Contact { Id = 6, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).ById(7), new Facts::Contact { Id = 7, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).ById(8), new Facts::Contact { Id = 8, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).ById(9), new Facts::Contact { Id = 9, HasWebsite = true });
        }

        [Test]
        public void ShouldTransformFirm()
        {
            SourceDb.Has(
                new Erm::Firm { Id = 1, Name = "firm", CreatedOn = Date, LastDisqualifyTime = Date.AddDays(1), ClientId = 2, OrganizationUnitId = 3, TerritoryId = 4, OwnerId = 5 });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.Firms.Map(x).ById(1), new Facts::Firm { Id = 1, Name = "firm", CreatedOn = Date, LastDisqualifiedOn = Date.AddDays(1), ClientId = 2, OrganizationUnitId = 3, TerritoryId = 4, OwnerId = 5 });
        }

        [Test]
        public void ShouldTransformFirmAddress()
        {
            SourceDb.Has(
                new Erm::FirmAddress { Id = 1, FirmId = 2 });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.FirmAddresses.Map(x).ById(1), new Facts::FirmAddress { Id = 1, FirmId = 2 });
        }

        [Test]
        public void ShouldTransformFirmContact()
        {
            const long NotNull = 123;

            SourceDb.Has(
                new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull },
                new Erm::FirmContact { Id = 2, ContactType = 1, FirmAddressId = null },
                new Erm::FirmContact { Id = 3, ContactType = 2, FirmAddressId = NotNull },
                new Erm::FirmContact { Id = 4, ContactType = 4, FirmAddressId = NotNull });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.FirmContacts.Map(x).ById(1), new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = NotNull })
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.FirmContacts.Map(x).ById(2), Enumerable.Empty<Facts::FirmContact>())
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.FirmContacts.Map(x).ById(3), Enumerable.Empty<Facts::FirmContact>())
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.FirmContacts.Map(x).ById(4), new Facts::FirmContact { Id = 4, HasWebsite = true, FirmAddressId = NotNull });
        }

        [Test]
        public void ShouldTransformLegalPerson()
        {
            SourceDb.Has(
                new Erm::LegalPerson { Id = 1, ClientId = 2 },
                new Erm::LegalPerson { Id = 2, ClientId = null });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.LegalPersons.Map(x).ById(1), new Facts::LegalPerson { Id = 1, ClientId = 2 })
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.LegalPersons.Map(x).ById(2), Enumerable.Empty<Facts::LegalPerson>());
        }

        [Test]
        public void ShouldTransformOrder()
        {
            SourceDb.Has(
                new Erm::Order { Id = 1, EndDistributionDateFact = Date, WorkflowStepId = 1, FirmId = 2 },
                new Erm::Order { Id = 2, EndDistributionDateFact = Date, WorkflowStepId = 4 /* on termination*/, FirmId = 2 });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.Orders.Map(x).ById(1), Enumerable.Empty<Facts::Order>())
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.Orders.Map(x).ById(2), new Facts::Order { Id = 2, EndDistributionDateFact = Date, FirmId = 2 });
        }

        [Test]
        public void ShouldTransformProject()
        {
            SourceDb.Has(
                new Erm::Project { Id = 1, Name = "name", OrganizationUnitId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Projects.Map(x).ById(1), new Facts::Project { Id = 1, Name = "name", OrganizationUnitId = 2 });
        }

        [Test]
        public void ShouldTransformTerritory()
        {
            SourceDb.Has(
                new Erm::Territory { Id = 1, Name = "name", OrganizationUnitId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Territories.Map(x).ById(1), new Facts::Territory { Id = 1, Name = "name", OrganizationUnitId = 2 });
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

            public Transformation VerifyTransform<T>(Func<IQuery, IEnumerable<T>> reader, params T[] expected)
            {
                VerifyTransform(reader, expected, x => x, null);
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