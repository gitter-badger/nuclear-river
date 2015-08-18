using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture, SetCulture("")]
    internal class ErmMapToFactsSpecsTests : TransformationFixtureBase
    {
        private static readonly DateTimeOffset Date = new DateTimeOffset(2015, 04, 03, 12, 30, 00, new TimeSpan());

        [Test]
        public void ShouldTransformAccount()
        {
            Transformation.Create(Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Erm::Account>>()) == Inquire(new Erm::Account { Id = 1, Balance = 123.45m, LegalPersonId = 2 })))
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.Accounts(new[] { 1L }).Map(q), Inquire(new Facts::Account { Id = 1, Balance = 123.45m, LegalPersonId = 2 }));
        }

        [Test]
        public void ShouldTransformBranchOfficeOrganizationUnit()
        {
            Transformation.Create(Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Erm::BranchOfficeOrganizationUnit>>()) == Inquire(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 })))
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.BranchOfficeOrganizationUnits(new[] { 1L }).Map(q),
                                           Inquire(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 }));
        }

        [Test]
        public void ShouldTransformCategory()
        {
            Transformation.Create(Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Erm::Category>>()) == Inquire(new Erm::Category { Id = 1, Level = 2, ParentId = 3 })))
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.Categories(new[] { 1L }).Map(q), Inquire(new Facts::Category { Id = 1, Level = 2, ParentId = 3 }));
        }

        [Test]
        public void ShouldTransformCategoryFirmAddress()
        {
            Transformation.Create(Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Erm::CategoryFirmAddress>>()) == Inquire(new Erm::CategoryFirmAddress { Id = 1, CategoryId = 2, FirmAddressId = 3 })))
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.CategoryFirmAddresses(new[] { 1L }).Map(q), Inquire(new Facts::CategoryFirmAddress { Id = 1, CategoryId = 2, FirmAddressId = 3 }));
        }

        [Test]
        public void ShouldTransformCategoryGroup()
        {
            Transformation.Create(Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Erm::CategoryGroup>>()) == Inquire(new Erm::CategoryGroup { Id = 1, Name = "name", Rate = 1 })))
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.CategoryGroups(new[] { 1L }).Map(q), Inquire(new Facts::CategoryGroup { Id = 1, Name = "name", Rate = 1 }));
        }

        [Test]
        public void ShouldTransformCategoryOrganizationUnit()
        {
            Transformation.Create(Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Erm::CategoryOrganizationUnit>>()) == Inquire(new Erm::CategoryOrganizationUnit { Id = 1, CategoryId = 2, CategoryGroupId = 3, OrganizationUnitId = 4 })))
                          .VerifyTransform(q => Specs.Erm.Map.ToFacts.CategoryOrganizationUnits(new[] { 1L }).Map(q), Inquire(new Facts::CategoryOrganizationUnit { Id = 1, CategoryId = 2, CategoryGroupId = 3, OrganizationUnitId = 4 }));
        }

        [Test]
        public void ShouldTransformClient()
        {
            var query = new Mock<IQuery>();
            query.Setup(q => q.For(It.IsAny<FindSpecification<Erm::Client>>()),
                        new Erm::Client { Id = 1, Name = "client", LastDisqualifyTime = Date },
                        new Erm::Client { Id = 2, MainPhoneNumber = "phone" },
                        new Erm::Client { Id = 3, AdditionalPhoneNumber1 = "phone" },
                        new Erm::Client { Id = 4, AdditionalPhoneNumber2 = "phone" },
                        new Erm::Client { Id = 5, Website = "site" });
            Transformation.Create(query.Object)
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Clients(new[] { 1L }).Map(x), Inquire(new Facts::Client { Id = 1, Name = "client", LastDisqualifiedOn = Date }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Clients(new[] { 2L }).Map(x), Inquire(new Facts::Client { Id = 2, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Clients(new[] { 3L }).Map(x), Inquire(new Facts::Client { Id = 3, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Clients(new[] { 4L }).Map(x), Inquire(new Facts::Client { Id = 4, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Clients(new[] { 5L }).Map(x), Inquire(new Facts::Client { Id = 5, HasWebsite = true }));
        }

        [Test]
        public void ShouldTransformContact()
        {
            var query = new Mock<IQuery>();
            query.Setup(q => q.For(It.IsAny<FindSpecification<Erm::Contact>>()),
                        new Erm::Contact { Id = 1, IsFired = true, ClientId = 2 },
                        new Erm::Contact { Id = 2, Role = 200000 },
                        new Erm::Contact { Id = 3, Role = 200001 },
                        new Erm::Contact { Id = 4, Role = 200002 },
                        new Erm::Contact { Id = 5, MainPhoneNumber = "phone" },
                        new Erm::Contact { Id = 6, MobilePhoneNumber = "phone" },
                        new Erm::Contact { Id = 7, HomePhoneNumber = "phone" },
                        new Erm::Contact { Id = 8, AdditionalPhoneNumber = "phone" },
                        new Erm::Contact { Id = 9, Website = "site" });

            Transformation.Create(query.Object)
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts(new[] { 1L }).Map(x), Inquire(new Facts::Contact { Id = 1, IsFired = true, ClientId = 2 }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts(new[] { 2L }).Map(x), Inquire(new Facts::Contact { Id = 2, Role = 1 }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts(new[] { 3L }).Map(x), Inquire(new Facts::Contact { Id = 3, Role = 2 }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts(new[] { 4L }).Map(x), Inquire(new Facts::Contact { Id = 4, Role = 3 }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts(new[] { 5L }).Map(x), Inquire(new Facts::Contact { Id = 5, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts(new[] { 6L }).Map(x), Inquire(new Facts::Contact { Id = 6, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts(new[] { 7L }).Map(x), Inquire(new Facts::Contact { Id = 7, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts(new[] { 8L }).Map(x), Inquire(new Facts::Contact { Id = 8, HasPhone = true }))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Contacts(new[] { 9L }).Map(x), Inquire(new Facts::Contact { Id = 9, HasWebsite = true }));
        }

        [Test]
        public void ShouldTransformFirm()
        {
            Transformation.Create(Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Firm>>()) == Inquire(
                new Erm::Firm { Id = 1, Name = "firm", CreatedOn = Date, LastDisqualifyTime = Date.AddDays(1), ClientId = 2, OrganizationUnitId = 3, TerritoryId = 4, OwnerId = 5 }
                )))
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.Firms(new[] { 1L }).Map(x), Inquire(new Facts::Firm { Id = 1, Name = "firm", CreatedOn = Date, LastDisqualifiedOn = Date.AddDays(1), ClientId = 2, OrganizationUnitId = 3, TerritoryId = 4, OwnerId = 5 }));
        }

        [Test]
        public void ShouldTransformFirmAddress()
        {
            Transformation.Create(Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::FirmAddress>>()) == Inquire(
                new Erm::FirmAddress { Id = 1, FirmId = 2 }
                )))
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.FirmAddresses(new[] { 1L }).Map(x), Inquire(new Facts::FirmAddress { Id = 1, FirmId = 2 }));
        }

        [Test]
        public void ShouldTransformFirmContact()
        {
            const long NotNull = 123;

            var query = new Mock<IQuery>();
            query.Setup(q => q.For(It.IsAny<FindSpecification<Erm::FirmContact>>()),
                        new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull },
                        new Erm::FirmContact { Id = 2, ContactType = 1, FirmAddressId = null },
                        new Erm::FirmContact { Id = 3, ContactType = 2, FirmAddressId = NotNull },
                        new Erm::FirmContact { Id = 4, ContactType = 4, FirmAddressId = NotNull });

            Transformation.Create(query.Object)
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.FirmContacts(new[] { 1L }).Map(x), Inquire(new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = NotNull }))
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.FirmContacts(new[] { 2L }).Map(x), Inquire<Facts::FirmContact>())
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.FirmContacts(new[] { 3L }).Map(x), Inquire<Facts::FirmContact>())
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.FirmContacts(new[] { 4L }).Map(x), Inquire(new Facts::FirmContact { Id = 4, HasWebsite = true, FirmAddressId = NotNull }));
        }

        [Test]
        public void ShouldTransformLegalPerson()
        {
            var query = new Mock<IQuery>();
            query.Setup(q => q.For(It.IsAny<FindSpecification<Erm::LegalPerson>>()),
                        new Erm::LegalPerson { Id = 1, ClientId = 2 },
                        new Erm::LegalPerson { Id = 2, ClientId = null });

            Transformation.Create(query.Object)
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.LegalPersons(new[] { 1L }).Map(x), Inquire(new Facts::LegalPerson { Id = 1, ClientId = 2 }))
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.LegalPersons(new[] { 2L }).Map(x), Inquire<Facts::LegalPerson>());
        }

        [Test]
        public void ShouldTransformOrder()
        {
            var query = new Mock<IQuery>();
            query.Setup(q => q.For(It.IsAny<FindSpecification<Erm::Order>>()),
                        new Erm::Order { Id = 1, EndDistributionDateFact = Date, WorkflowStepId = 1, FirmId = 2 },
                        new Erm::Order { Id = 2, EndDistributionDateFact = Date, WorkflowStepId = 4 /* on termination*/, FirmId = 2 });

            Transformation.Create(query.Object)
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.Orders(new[] { 1L }).Map(x), Inquire<Facts::Order>())
                .VerifyTransform(x => Specs.Erm.Map.ToFacts.Orders(new[] { 2L }).Map(x), Inquire(new Facts::Order { Id = 2, EndDistributionDateFact = Date, FirmId = 2 }));
        }

        [Test]
        public void ShouldTransformProject()
        {
            Transformation.Create(Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Project>>()) == Inquire(new Erm::Project { Id = 1, Name = "name", OrganizationUnitId = 2 })))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Projects(new[] { 1L }).Map(x), Inquire(new Facts::Project { Id = 1, Name = "name", OrganizationUnitId = 2 }));
        }

        [Test]
        public void ShouldTransformTerritory()
        {
            Transformation.Create(Mock.Of<IQuery>(query => query.For(It.IsAny<FindSpecification<Erm::Territory>>()) == Inquire(new Erm::Territory { Id = 1, Name = "name", OrganizationUnitId = 2 })))
                          .VerifyTransform(x => Specs.Erm.Map.ToFacts.Territories(new[] { 1L }).Map(x), Inquire(new Facts::Territory { Id = 1, Name = "name", OrganizationUnitId = 2 }));
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