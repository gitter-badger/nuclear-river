using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture, SetCulture("")]
    internal class CustomerIntelligenceTransformationContextTests : BaseTransformationFixture
    {
        [Test]
        public void ShouldTransformCategoryGroup()
        {
            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::CategoryGroup>()).
                Returns(Inquire(new Facts::CategoryGroup { Id = 123, Name = "category group", Rate = 1 }));

            Transformation.Create(mock.Object)
                          .VerifyTransform(x => x.CategoryGroups.ById(123), Inquire(new CI::CategoryGroup { Id = 123, Name = "category group", Rate = 1 }));
        }

        [Test]
        public void ShouldTransformClient()
        {
            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::Client>())
                .Returns(Inquire(new Facts::Client { Id = 1, Name = "a client" }));

            Transformation.Create(mock.Object)
                .VerifyTransform(x => x.Clients.ById(1), Inquire(new CI::Client { Name = "a client" }), x => new { x.Name }, "The name should be processed.");
        }

        [Test]
        public void ShouldTransformClientContact()
        {
            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::Contact>())
                .Returns(Inquire(new Facts::Contact { Id = 1, Role = 1 },
                new Facts::Contact { Id = 2, IsFired = true },
                                 new Facts::Contact { Id = 3, ClientId = 1 }));

            Transformation.Create(mock.Object)
                .VerifyTransform(x => x.ClientContacts.Where(y => y.ContactId == 1), Inquire(new CI::ClientContact { Role = 1 }), x => new { x.Role }, "The role should be processed.")
                .VerifyTransform(x => x.ClientContacts.Where(y => y.ContactId == 2), Inquire(new CI::ClientContact { IsFired = true }), x => new { x.IsFired }, "The IsFired should be processed.")
                .VerifyTransform(x => x.ClientContacts.Where(y => y.ContactId == 3), Inquire(new CI::ClientContact { ClientId = 1 }), x => new { x.ClientId }, "The client reference should be processed.");
        }

        [Test]
        public void ShouldTransformFirm()
        {
            var now = DateTimeOffset.UtcNow;
            var dayAgo = now.AddDays(-1);
            var monthAgo = now.AddMonths(-1);

            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::Project>())
                .Returns(Inquire(new Facts::Project { Id = 1, OrganizationUnitId = 1 },
                                 new Facts::Project { Id = 2, OrganizationUnitId = 2 }));
            mock.Setup(x => x.For<Facts::Firm>())
                .Returns(Inquire(
                new Facts::Firm { Id = 1, Name = "1st firm", CreatedOn = monthAgo, LastDisqualifiedOn = dayAgo, OrganizationUnitId = 1, TerritoryId = 1 },
                    new Facts::Firm { Id = 2, Name = "2nd firm", CreatedOn = monthAgo, LastDisqualifiedOn = dayAgo, ClientId = 1, OrganizationUnitId = 2, TerritoryId = 2 }));
            mock.Setup(x => x.For<Facts::FirmAddress>())
                .Returns(Inquire(new Facts::FirmAddress { Id = 1, FirmId = 1 },
                                 new Facts::FirmAddress { Id = 2, FirmId = 1 }));
            mock.Setup(x => x.For<Facts::Client>())
                .Returns(Inquire(new Facts::Client { Id = 1, LastDisqualifiedOn = now }));
            mock.Setup(x => x.For<Facts::LegalPerson>())
                .Returns(Inquire(new Facts::LegalPerson { Id = 1, ClientId = 1 }));
            mock.Setup(x => x.For<Facts::Order>())
                .Returns(Inquire(new Facts::Order { FirmId = 1, EndDistributionDateFact = dayAgo }));

            // TODO: split into several tests
            Transformation.Create(mock.Object)
                .VerifyTransform(x => x.Firms.ById(1), Inquire(new CI::Firm { Name = "1st firm" }), x => new { x.Name }, "The name should be processed.")
                .VerifyTransform(x => x.Firms.ById(1), Inquire(new CI::Firm { CreatedOn = monthAgo }), x => new { x.CreatedOn }, "The createdOn should be processed.")
                          .VerifyTransform(x => x.Firms.ById(1, 2),
                                           Inquire(new CI::Firm { LastDisqualifiedOn = dayAgo },
                                                   new CI::Firm { LastDisqualifiedOn = now }),
                                           x => new { x.LastDisqualifiedOn },
                                           "The disqualifiedOn should be processed.")
                          .VerifyTransform(x => x.Firms.ById(1, 2),
                                           Inquire(new CI::Firm { LastDistributedOn = dayAgo },
                                                   new CI::Firm { LastDistributedOn = null }),
                                           x => new { x.LastDistributedOn },
                                           "The distributedOn should be processed.")
                          .VerifyTransform(x => x.Firms.ById(1, 2),
                                           Inquire(new CI::Firm { AddressCount = 2 },
                                                   new CI::Firm { AddressCount = 0 }),
                                           x => new { x.AddressCount },
                                           "The address count should be processed.")
                          .VerifyTransform(x => x.Firms.ById(1, 2),
                                           Inquire(new CI::Firm { Id = 1, ClientId = null, ProjectId = 1, TerritoryId = 1 },
                                                   new CI::Firm { Id = 2, ClientId = 1, ProjectId = 2, TerritoryId = 2 }),
                                           x => new { x.Id, x.ClientId, x.ProjectId, x.TerritoryId },
                                           "The references should be processed.");
        }

        [Test]
        public void ShouldTransformFirmContactInfoFromClient()
        {
            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::Project>())
                .Returns(Inquire(new Facts::Project { Id = 1, OrganizationUnitId = 0 }));
            mock.Setup(x => x.For<Facts::Firm>())
                .Returns(Inquire(new Facts::Firm { Id = 1, },
                new Facts::Firm { Id = 2, ClientId = 1 },
                new Facts::Firm { Id = 3, ClientId = 2 },
                                 new Facts::Firm { Id = 4, ClientId = 3 }));
            mock.Setup(x => x.For<Facts::Client>())
                .Returns(Inquire(new Facts::Client { Id = 1, HasPhone = true, HasWebsite = true },
                new Facts::Client { Id = 2, HasPhone = false, HasWebsite = false },
                                 new Facts::Client { Id = 3, HasPhone = false, HasWebsite = false }));
            mock.Setup(x => x.For<Facts::Contact>())
                .Returns(Inquire(new Facts::Contact { Id = 1, ClientId = 2, HasPhone = true, HasWebsite = true },
                new Facts::Contact { Id = 2, ClientId = 3, HasPhone = true, HasWebsite = false },
                                 new Facts::Contact { Id = 3, ClientId = 3, HasPhone = false, HasWebsite = true }));

            Transformation.Create(mock.Object)
                .VerifyTransform(x => x.Firms.ById(1), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => x.Firms.ById(2), Inquire(new CI::Firm { HasPhone = true, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => x.Firms.ById(3), Inquire(new CI::Firm { HasPhone = true, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => x.Firms.ById(4), Inquire(new CI::Firm { HasPhone = true, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite });
        }

        [Test]
        public void ShouldTransformFirmContactInfoFromFirm()
        {
            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::Project>())
                .Returns(Inquire(new Facts::Project { Id = 1, OrganizationUnitId = 0 }));
            mock.Setup(x => x.For<Facts::Firm>())
                .Returns(Inquire(new Facts::Firm { Id = 1, Name = "has no addresses" },
                new Facts::Firm { Id = 2, Name = "has addresses, but no contacts" },
                new Facts::Firm { Id = 3, Name = "has one phone contact" },
                new Facts::Firm { Id = 4, Name = "has one website contact" },
                                 new Facts::Firm { Id = 5, Name = "has an unknown contact" }));
            mock.Setup(x => x.For<Facts::FirmAddress>())
                .Returns(Inquire(new Facts::FirmAddress { Id = 1, FirmId = 2 },
                new Facts::FirmAddress { Id = 2, FirmId = 3 },
                new Facts::FirmAddress { Id = 3, FirmId = 4 },
                                 new Facts::FirmAddress { Id = 4, FirmId = 5 }));
            mock.Setup(x => x.For<Facts::FirmContact>())
                .Returns(Inquire(new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = 2 },
                new Facts::FirmContact { Id = 2, HasWebsite = true, FirmAddressId = 3 },
                                 new Facts::FirmContact { Id = 3, FirmAddressId = 4 }));

            Transformation.Create(mock.Object)
                .VerifyTransform(x => x.Firms.ById(1), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => x.Firms.ById(2), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => x.Firms.ById(3), Inquire(new CI::Firm { HasPhone = true, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => x.Firms.ById(4), Inquire(new CI::Firm { HasPhone = false, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => x.Firms.ById(5), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite });
        }

        [Test]
        public void ShouldTransformFirmBalance()
        {
            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::Firm>())
                .Returns(Inquire(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 2 },
                                 new Facts::Firm { Id = 3, ClientId = 1, OrganizationUnitId = 1 }));
            mock.Setup(x => x.For<Facts::Client>())
                .Returns(Inquire(new Facts::Client { Id = 1 },
                                 new Facts::Client { Id = 2 }));
            mock.Setup(x => x.For<Facts::LegalPerson>())
                .Returns(Inquire(new Facts::LegalPerson { Id = 1, ClientId = 1 },
                                 new Facts::LegalPerson { Id = 2, ClientId = 2 }));
            mock.Setup(x => x.For<Facts::BranchOfficeOrganizationUnit>())
                .Returns(Inquire(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 },
                                 new Facts::BranchOfficeOrganizationUnit { Id = 2, OrganizationUnitId = 2 }));
            mock.Setup(x => x.For<Facts::Account>())
                .Returns(Inquire(new Facts::Account { Id = 1, Balance = 123, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 },
                new Facts::Account { Id = 2, Balance = 234, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 2 },
                                 new Facts::Account { Id = 3, Balance = 345, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 2 }));

            Transformation.Create(mock.Object)
                          .VerifyTransform(x => x.FirmBalances.OrderBy(fb => fb.FirmId),
                                           Inquire(new CI::FirmBalance { FirmId = 1, Balance = 123, AccountId = 1 },
                    new CI::FirmBalance { FirmId = 2, Balance = 345, AccountId = 3 },
                                                   new CI::FirmBalance { FirmId = 3, Balance = 123, AccountId = 1 }),
                                           "The balance should be processed.");
        }

        [Test]
        public void ShouldTransformFirmCategory()
        {
            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::Category>())
                .Returns(Inquire(new Facts::Category { Id = 1, Level = 1 },
                        new Facts::Category { Id = 2, Level = 2, ParentId = 1 }, 
                        new Facts::Category { Id = 3, Level = 3, ParentId = 2 },
                        new Facts::Category { Id = 4, Level = 3, ParentId = 2 }));
            mock.Setup(x => x.For<Facts::Firm>())
                .Returns(Inquire(new Facts::Firm { Id = 1 }));
            mock.Setup(x => x.For<Facts::FirmAddress>())
                .Returns(Inquire(new Facts::FirmAddress { Id = 1, FirmId = 1 },
                                 new Facts::FirmAddress { Id = 2, FirmId = 1 }));
            mock.Setup(x => x.For<Facts::CategoryFirmAddress>())
                .Returns(Inquire(new Facts::CategoryFirmAddress { FirmAddressId = 1, CategoryId = 3 },
                                 new Facts::CategoryFirmAddress { FirmAddressId = 2, CategoryId = 4 }));
            mock.Setup(x => x.For<Facts::FirmCategoryStatistics>())
                .Returns(Inquire(new Facts::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, Hits = 1, Shows = 1 },
                new Facts::FirmCategoryStatistics { FirmId = 1, CategoryId = 2, Hits = 2 },
                                 new Facts::FirmCategoryStatistics { FirmId = 1, CategoryId = 3, Shows = 2 }));

            Transformation.Create(mock.Object)
                          .VerifyTransform(x => x.FirmCategories,
                                           Inquire(new CI::FirmCategory { FirmId = 1, CategoryId = 1 },
                    new CI::FirmCategory { FirmId = 1, CategoryId = 2 },
                    new CI::FirmCategory { FirmId = 1, CategoryId = 3 },
                                                   new CI::FirmCategory { FirmId = 1, CategoryId = 4 }),
                                           "The firm categories should be processed.");
        }

        [Test]
        public void ShouldTransformProject()
        {
            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::Project>())
                .Returns(Inquire(new Facts::Project { Id = 123, Name = "p1" },
                                 new Facts::Project { Id = 456, Name = "p2", OrganizationUnitId = 1 }));

            Transformation.Create(mock.Object)
                          .VerifyTransform(x => x.Projects,
                                           Inquire(new CI::Project { Id = 123, Name = "p1" },
                                                   new CI::Project { Id = 456, Name = "p2" }),
                                           "The projects should be processed.");
        }

        [Test]
        public void ShouldTransformProjectCategory()
        {
            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::Project>())
                .Returns(Inquire(new Facts::Project { Id = 1, OrganizationUnitId = 2 }));
            mock.Setup(x => x.For<Facts::CategoryOrganizationUnit>())
                .Returns(Inquire(new Facts::CategoryOrganizationUnit { OrganizationUnitId = 2, CategoryId = 3 },
                                                                               new Facts::CategoryOrganizationUnit { OrganizationUnitId = 2, CategoryId = 4 }));
            mock.Setup(x => x.For<Facts::Category>())
                .Returns(Inquire(new Facts::Category { Id = 3 },
                                                                new Facts::Category { Id = 4 }));
            
            // Десять фирм в проекте, каждая с рубрикой #3
            mock.Setup(x => x.For<Facts::Firm>())
                .Returns(Inquire(Enumerable.Range(0, 10).Select(i => new Facts::Firm { Id = i, OrganizationUnitId = 2 }).ToArray()));
            mock.Setup(x => x.For<Facts::FirmAddress>())
                .Returns(Inquire(Enumerable.Range(0, 10).Select(i => new Facts::FirmAddress { Id = i, FirmId = i }).ToArray()));
            mock.Setup(x => x.For<Facts::CategoryFirmAddress>())
                .Returns(Inquire(Enumerable.Range(0, 10).Select(i => new Facts::CategoryFirmAddress { Id = i, FirmAddressId = i, CategoryId = 3 }).ToArray()));

            mock.Setup(x => x.For<Facts::ProjectCategoryStatistics>())
                .Returns(Inquire(new Facts.ProjectCategoryStatistics { ProjectId = 1, AdvertisersCount = 1, CategoryId = 3 }));

            Transformation.Create(mock.Object)
                          .VerifyTransform(x => x.ProjectCategories,
                                           Inquire(new CI::ProjectCategory { ProjectId = 1, CategoryId = 3 },
                                                                             new CI::ProjectCategory { ProjectId = 1, CategoryId = 4 }));
        }

        [Test]
        public void ShouldTransformTerritories()
        {
            var mock = new Mock<IQuery>();
            mock.Setup(x => x.For<Facts::Project>())
                .Returns(Inquire(new Facts::Project { Id = 1, OrganizationUnitId = 1 },
                        new Facts::Project { Id = 2, OrganizationUnitId = 2 }));
            mock.Setup(x => x.For<Facts::Territory>())
                .Returns(Inquire(new Facts::Territory { Id = 1, Name = "name1", OrganizationUnitId = 1 },
                        new Facts::Territory { Id = 2, Name = "name2", OrganizationUnitId = 2 }));

            Transformation.Create(mock.Object)
                          .VerifyTransform(x => x.Territories,
                                           Inquire(
                    new CI::Territory { Id = 1, Name = "name1", ProjectId = 1 },
                                               new CI::Territory { Id = 2, Name = "name2", ProjectId = 2 }));
        }

        #region Transformation

        private class Transformation
        {
            private readonly ICustomerIntelligenceContext _transformation;

            private Transformation(IQuery source)
            {
                _transformation = new CustomerIntelligenceTransformationContext(source);
            }

            public static Transformation Create(IQuery source = null)
            {
                return new Transformation(source ?? new Mock<IQuery>().Object);
            }

            public Transformation VerifyTransform<T>(Func<ICustomerIntelligenceContext, IEnumerable<T>> reader, IEnumerable<T> expected, string message = null)
            {
                VerifyTransform(reader, expected, x => x, message);
                return this;
            }

            public Transformation VerifyTransform<T, TProjection>(Func<ICustomerIntelligenceContext, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                // TODO: convert to a custom NUnit constraint, at least for fail logging
                Assert.That(reader(_transformation), Is.EquivalentTo(expected).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }

        #endregion
    }
}