using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture, SetCulture("")]
    internal class FactsMapToCISpecsTests : TransformationFixtureBase
    {
        [Test]
        public void ShouldTransformCategoryGroup()
        {
            var query = new MemoryMockQuery(
                new Facts::CategoryGroup { Id = 123, Name = "category group", Rate = 1 });

            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.CategoryGroups.Map(x).ById(123), Inquire(new CI::CategoryGroup { Id = 123, Name = "category group", Rate = 1 }));
        }

        [Test]
        public void ShouldTransformClient()
        {
            var query = new MemoryMockQuery(
                new Facts::Client { Id = 1, Name = "a client" });

            Transformation.Create(query)
                .VerifyTransform(x => Specs.Facts.Map.ToCI.Clients.Map(x).ById(1), Inquire(new CI::Client { Name = "a client" }), x => new { x.Name }, "The name should be processed.");
        }

        [Test]
        public void ShouldTransformClientContact()
        {
            var query = new MemoryMockQuery(
                new Facts::Contact { ClientId = 1, Role = 1 },
                new Facts::Contact { ClientId = 2, IsFired = true },
                new Facts::Contact { ClientId = 3 });

            Transformation.Create(query)
                .VerifyTransform(x => Specs.Facts.Map.ToCI.ClientContacts.Map(x).Where(c => c.ClientId == 1), Inquire(new CI::ClientContact { Role = 1 }), x => new { x.Role }, "The role should be processed.")
                .VerifyTransform(x => Specs.Facts.Map.ToCI.ClientContacts.Map(x).Where(c => c.ClientId == 2), Inquire(new CI::ClientContact { IsFired = true }), x => new { x.IsFired }, "The IsFired should be processed.")
                .VerifyTransform(x => Specs.Facts.Map.ToCI.ClientContacts.Map(x).Where(c => c.ClientId == 3), Inquire(new CI::ClientContact { ClientId = 3 }), x => new { x.ClientId }, "The client reference should be processed.");
        }

        [Test]
        public void ShouldTransformFirm()
        {
            var now = DateTimeOffset.UtcNow;
            var dayAgo = now.AddDays(-1);
            var monthAgo = now.AddMonths(-1);

            var query = new MemoryMockQuery(
                new Facts::Project { Id = 1, OrganizationUnitId = 1 },
                new Facts::Project { Id = 2, OrganizationUnitId = 2 },
                new Facts::Firm { Id = 1, Name = "1st firm", CreatedOn = monthAgo, LastDisqualifiedOn = dayAgo, OrganizationUnitId = 1, TerritoryId = 1 },
                new Facts::Firm { Id = 2, Name = "2nd firm", CreatedOn = monthAgo, LastDisqualifiedOn = dayAgo, ClientId = 1, OrganizationUnitId = 2, TerritoryId = 2 },
                new Facts::FirmAddress { Id = 1, FirmId = 1 },
                new Facts::FirmAddress { Id = 2, FirmId = 1 },
                new Facts::Client { Id = 1, LastDisqualifiedOn = now },
                new Facts::LegalPerson { Id = 1, ClientId = 1 },
                new Facts::Order { FirmId = 1, EndDistributionDateFact = dayAgo });

            // TODO: split into several tests
            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(1),
                                           Inquire(new CI::Firm { Name = "1st firm" }),
                                           x => new { x.Name },
                                           "The name should be processed.")
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(1),
                                           Inquire(new CI::Firm { CreatedOn = monthAgo }),
                                           x => new { x.CreatedOn },
                                           "The createdOn should be processed.")
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(1, 2),
                                           Inquire(new CI::Firm { LastDisqualifiedOn = dayAgo },
                                                   new CI::Firm { LastDisqualifiedOn = now }),
                                           x => new { x.LastDisqualifiedOn },
                                           "The disqualifiedOn should be processed.")
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(1, 2),
                                           Inquire(new CI::Firm { LastDistributedOn = dayAgo },
                                                   new CI::Firm { LastDistributedOn = null }),
                                           x => new { x.LastDistributedOn },
                                           "The distributedOn should be processed.")
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(1, 2),
                                           Inquire(new CI::Firm { AddressCount = 2 },
                                                   new CI::Firm { AddressCount = 0 }),
                                           x => new { x.AddressCount },
                                           "The address count should be processed.")
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(1, 2),
                                           Inquire(new CI::Firm { Id = 1, ClientId = null, ProjectId = 1, TerritoryId = 1 },
                                                   new CI::Firm { Id = 2, ClientId = 1, ProjectId = 2, TerritoryId = 2 }),
                                           x => new { x.Id, x.ClientId, x.ProjectId, x.TerritoryId },
                                           "The references should be processed.");
        }

        [Test]
        public void ShouldTransformFirmContactInfoFromClient()
        {
            var query = new MemoryMockQuery(
                new Facts::Project { Id = 1, OrganizationUnitId = 0 },
                new Facts::Firm { Id = 1, },
                new Facts::Firm { Id = 2, ClientId = 1 },
                new Facts::Firm { Id = 3, ClientId = 2 },
                new Facts::Firm { Id = 4, ClientId = 3 },
                new Facts::Client { Id = 1, HasPhone = true, HasWebsite = true },
                new Facts::Client { Id = 2, HasPhone = false, HasWebsite = false },
                new Facts::Client { Id = 3, HasPhone = false, HasWebsite = false },
                new Facts::Contact { Id = 1, ClientId = 2, HasPhone = true, HasWebsite = true },
                new Facts::Contact { Id = 2, ClientId = 3, HasPhone = true, HasWebsite = false },
                new Facts::Contact { Id = 3, ClientId = 3, HasPhone = false, HasWebsite = true });

            Transformation.Create(query)
                .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(1), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(2), Inquire(new CI::Firm { HasPhone = true, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(3), Inquire(new CI::Firm { HasPhone = true, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(4), Inquire(new CI::Firm { HasPhone = true, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite });
        }

        [Test]
        public void ShouldTransformFirmContactInfoFromFirm()
        {
            var query = new MemoryMockQuery(
                new Facts::Project { Id = 1, OrganizationUnitId = 0 },
                new Facts::Firm { Id = 1, Name = "has no addresses" },
                new Facts::Firm { Id = 2, Name = "has addresses, but no contacts" },
                new Facts::Firm { Id = 3, Name = "has one phone contact" },
                new Facts::Firm { Id = 4, Name = "has one website contact" },
                new Facts::Firm { Id = 5, Name = "has an unknown contact" },
                new Facts::FirmAddress { Id = 1, FirmId = 2 },
                new Facts::FirmAddress { Id = 2, FirmId = 3 },
                new Facts::FirmAddress { Id = 3, FirmId = 4 },
                new Facts::FirmAddress { Id = 4, FirmId = 5 },
                new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = 2 },
                new Facts::FirmContact { Id = 2, HasWebsite = true, FirmAddressId = 3 },
                new Facts::FirmContact { Id = 3, FirmAddressId = 4 });

            Transformation.Create(query)
                .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(1), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(2), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(3), Inquire(new CI::Firm { HasPhone = true, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(4), Inquire(new CI::Firm { HasPhone = false, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Facts.Map.ToCI.Firms.Map(x).ById(5), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite });
        }

        [Test]
        public void ShouldTransformFirmBalance()
        {
            var query = new MemoryMockQuery(
                new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 2 },
                new Facts::Firm { Id = 3, ClientId = 1, OrganizationUnitId = 1 },
                new Facts::Client { Id = 1 },
                new Facts::Client { Id = 2 },
                new Facts::LegalPerson { Id = 1, ClientId = 1 },
                new Facts::LegalPerson { Id = 2, ClientId = 2 },
                new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 },
                new Facts::BranchOfficeOrganizationUnit { Id = 2, OrganizationUnitId = 2 },
                new Facts::Account { Id = 1, Balance = 123, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 },
                new Facts::Account { Id = 2, Balance = 234, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 2 },
                new Facts::Account { Id = 3, Balance = 345, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 2 });

            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.FirmBalances.Map(x).Where(b => new long[] { 1, 2, 3 }.Contains(b.FirmId)).OrderBy(fb => fb.FirmId),
                                           Inquire(new CI::FirmBalance { FirmId = 1, Balance = 123, AccountId = 1 },
                                                   new CI::FirmBalance { FirmId = 2, Balance = 345, AccountId = 3 },
                                                   new CI::FirmBalance { FirmId = 3, Balance = 123, AccountId = 1 }),
                                           "The balance should be processed.");
        }

        [Test]
        public void ShouldTransformFirmCategory()
        {
            var query = new MemoryMockQuery(
                new Facts::Category { Id = 1, Level = 1 },
                new Facts::Category { Id = 2, Level = 2, ParentId = 1 },
                new Facts::Category { Id = 3, Level = 3, ParentId = 2 },
                new Facts::Category { Id = 4, Level = 3, ParentId = 2 },
                new Facts::Firm { Id = 1 },
                new Facts::FirmAddress { Id = 1, FirmId = 1 },
                new Facts::FirmAddress { Id = 2, FirmId = 1 },
                new Facts::CategoryFirmAddress { FirmAddressId = 1, CategoryId = 3 },
                new Facts::CategoryFirmAddress { FirmAddressId = 2, CategoryId = 4 },
                new Facts::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, Hits = 1, Shows = 1 },
                new Facts::FirmCategoryStatistics { FirmId = 1, CategoryId = 2, Hits = 2 },
                new Facts::FirmCategoryStatistics { FirmId = 1, CategoryId = 3, Shows = 2 });

            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.FirmCategories.Map(x).Where(c => c.FirmId == 1),
                                           Inquire(new CI::FirmCategory { FirmId = 1, CategoryId = 1 },
                                                   new CI::FirmCategory { FirmId = 1, CategoryId = 2 },
                                                   new CI::FirmCategory { FirmId = 1, CategoryId = 3 },
                                                   new CI::FirmCategory { FirmId = 1, CategoryId = 4 }),
                                           "The firm categories should be processed.");
        }

        [Test]
        public void ShouldTransformProject()
        {
            var query = new MemoryMockQuery(
                new Facts::Project { Id = 123, Name = "p1" },
                new Facts::Project { Id = 456, Name = "p2", OrganizationUnitId = 1 });

            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.Projects.Map(x).ById(123, 456),
                                           Inquire(new CI::Project { Id = 123, Name = "p1" },
                                                   new CI::Project { Id = 456, Name = "p2" }),
                                           "The projects should be processed.");
        }

        [Test]
        public void ShouldTransformProjectCategory()
        {
            var query = new MemoryMockQuery(
                new Facts::Project { Id = 1, OrganizationUnitId = 2 },
                new Facts::CategoryOrganizationUnit { OrganizationUnitId = 2, CategoryId = 3 },
                new Facts::CategoryOrganizationUnit { OrganizationUnitId = 2, CategoryId = 4 },
                new Facts::Category { Id = 3 },
                new Facts::Category { Id = 4 },
                new Facts::ProjectCategoryStatistics { ProjectId = 1, AdvertisersCount = 1, CategoryId = 3 });

            // Десять фирм в проекте, каждая с рубрикой #3
            query.AddRange(Enumerable.Range(0, 10).Select(i => new Facts::Firm { Id = i, OrganizationUnitId = 2 }));
            query.AddRange(Enumerable.Range(0, 10).Select(i => new Facts::FirmAddress { Id = i, FirmId = i }));
            query.AddRange(Enumerable.Range(0, 10).Select(i => new Facts::CategoryFirmAddress { Id = i, FirmAddressId = i, CategoryId = 3 }));

            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.ProjectCategories.Map(x).Where(c => c.ProjectId == 1),
                                           Inquire(new CI::ProjectCategory { ProjectId = 1, CategoryId = 3 },
                                                   new CI::ProjectCategory { ProjectId = 1, CategoryId = 4 }));
        }

        [Test]
        public void ShouldTransformTerritories()
        {
            var query = new MemoryMockQuery(
                new Facts::Project { Id = 1, OrganizationUnitId = 1 },
                new Facts::Project { Id = 2, OrganizationUnitId = 2 },
                new Facts::Territory { Id = 1, Name = "name1", OrganizationUnitId = 1 },
                new Facts::Territory { Id = 2, Name = "name2", OrganizationUnitId = 2 });

            Transformation.Create(query)
                          .VerifyTransform(x => Specs.Facts.Map.ToCI.Territories.Map(x).ById(1, 2),
                                           Inquire(new CI::Territory { Id = 1, Name = "name1", ProjectId = 1 },
                                                   new CI::Territory { Id = 2, Name = "name2", ProjectId = 2 }));
        }

        #region Transformation

        private class Transformation
        {
            private readonly IQuery _query;

            private Transformation(IQuery query)
            {
                _query = query;
            }

            public static Transformation Create(IQuery source = null)
            {
                return new Transformation(source ?? new Mock<IQuery>().Object);
            }

            public Transformation VerifyTransform<T>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, string message = null)
            {
                VerifyTransform(reader, expected, x => x, message);
                return this;
            }

            public Transformation VerifyTransform<T, TProjection>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                // TODO: convert to a custom NUnit constraint, at least for fail logging
                Assert.That(reader(_query).ToArray, Is.EquivalentTo(expected).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }

        #endregion
    }
}