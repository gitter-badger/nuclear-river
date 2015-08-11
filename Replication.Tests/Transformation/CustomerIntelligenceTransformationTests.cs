using System;
using System.Linq.Expressions;

using Moq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal class CustomerIntelligenceTransformationTests : BaseTransformationFixture
    {
        [Test]
        public void ShouldInitializeClient()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For(It.IsAny<FindSpecification<Facts::Client>>()),
                         new Facts::Client { Id = 1 });

            Transformation.Create(source.Object)
                .Transform(Aggregate.Initialize<CI::Client>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Client { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeClientHavingContact()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For(It.IsAny<FindSpecification<Facts::Client>>()),
                         new Facts::Client { Id = 1 })
                  .Setup(q => q.For<Facts::Contact>(),
                         new Facts::Contact { Id = 1, ClientId = 1 });

            Transformation.Create(source.Object)
                 .Transform(Aggregate.Initialize<CI::Client>(1))
                 .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                 .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 1, ContactId = 1 }))));
        }

        [Test]
        public void ShouldRecalculateClient()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For(It.IsAny<FindSpecification<Facts::Client>>()),
                         new Facts::Client { Id = 1, Name = "new name" });
            
            var target = new Mock<IQuery>();
            target.Setup(q => q.For(It.IsAny<FindSpecification<CI::Client>>()),
                         new CI::Client { Id = 1, Name = "old name" });

            Transformation.Create(source.Object, target.Object)
                .Transform(Aggregate.Recalculate<CI::Client>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldRecalculateClientHavingContact()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For(It.IsAny<FindSpecification<Facts::Client>>()),
                         new Facts::Client { Id = 1 },
                         new Facts::Client { Id = 2 },
                         new Facts::Client { Id = 3 })
                  .Setup(q => q.For<Facts::Contact>(),
                         new Facts::Contact { Id = 1, ClientId = 1},
                         new Facts::Contact { Id = 2, ClientId = 2 });

            var target = new Mock<IQuery>();
            target.Setup(q => q.For(It.IsAny<FindSpecification<CI::Client>>()),
                         new CI::Client { Id = 1 },
                         new CI::Client { Id = 2 },
                         new CI::Client { Id = 3 })
                  .Setup(q => q.For<CI::ClientContact>(),
                         new CI::ClientContact { ClientId = 2, ContactId = 2 },
                         new CI::ClientContact { ClientId = 3, ContactId = 3 });

            Transformation.Create(source.Object, target.Object)
                          .Transform(Aggregate.Recalculate<CI::Client>(1),
                                     Aggregate.Recalculate<CI::Client>(2),
                                     Aggregate.Recalculate<CI::Client>(3))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 2 }))))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 3 }))))
                          .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 1, ContactId = 1 }))))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 2, ContactId = 2 }))))
                          .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 3, ContactId = 3 }))));
        }

        [Test]
        public void ShouldDestroyClient()
        {
            var source = Mock.Of<IQuery>();
            var target = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Client>>()) == Inquire(new CI::Client { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Client>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Client { Id = 1 }))));
        }

        [Test]
        public void ShouldDestroyClientHavingContact()
        {
            var source = Mock.Of<IQuery>();
            var target = Mock.Of<IQuery>(q =>
                q.For(It.IsAny<FindSpecification<CI::Client>>()) == Inquire(new CI::Client { Id = 1 }) &&
                q.For<CI::ClientContact>() == Inquire(new CI::ClientContact { ClientId = 1, ContactId = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Client>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 1, ContactId = 1 }))));
        }

        [Test]
        public void ShouldInitializeFirm()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(source.Object)
                          .Transform(Aggregate.Initialize<CI::Firm>(1))
                          .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingBalance()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::Client>(),
                         new Facts::Client { Id = 1 })
                  .Setup(q => q.For<Facts::BranchOfficeOrganizationUnit>(),
                         new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::LegalPerson>(),
                         new Facts::LegalPerson { Id = 1, ClientId = 1 })
                  .Setup(q => q.For<Facts::Account>(),
                         new Facts::Account { LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1, Balance = 123.45m })
                  .Setup(q => q.For<Facts::Firm>(),
                         q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(source.Object)
                .Transform(Aggregate.Initialize<CI::Firm>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 1, Balance = 123.45m }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingCategory()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For<Facts::Category>(),
                         new Facts::Category { Id = 1, Level = 3 })
                  .Setup(q => q.For<Facts::CategoryOrganizationUnit>(),
                         new Facts::CategoryOrganizationUnit { CategoryId = 1, OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::FirmAddress>(),
                         new Facts::FirmAddress { Id = 1, FirmId = 1 })
                  .Setup(q => q.For<Facts::CategoryFirmAddress>(),
                         new Facts::CategoryFirmAddress { FirmAddressId = 1, CategoryId = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(source.Object)
                .Transform(Aggregate.Initialize<CI::Firm>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1, AddressCount = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingClient()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For<Facts::Category>(),
                         new Facts::Category { Id = 1, Level = 3 })
                  .Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::Client>(),
                         new Facts::Client { Id = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

           Transformation.Create(source.Object)
                .Transform(Aggregate.Initialize<CI::Firm>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldRecalculateFirm()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, OrganizationUnitId = 1 });
            var target = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1 }));

            Transformation.Create(source.Object, target)
                .Transform(Aggregate.Recalculate<CI::Firm>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingBalance()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::Client>(),
                         new Facts::Client { Id = 1 },
                         new Facts::Client { Id = 2 })
                  .Setup(q => q.For<Facts::BranchOfficeOrganizationUnit>(),
                         new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::LegalPerson>(),
                         new Facts::LegalPerson { Id = 1, ClientId = 1 },
                         new Facts::LegalPerson { Id = 2, ClientId = 2 })
                  .Setup(q => q.For<Facts::Account>(),
                         new Facts::Account { LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1, Balance = 123 },
                         new Facts::Account { LegalPersonId = 2, BranchOfficeOrganizationUnitId = 1, Balance = 456 })
                  .Setup(q => q.For<Facts::Firm>(),
                         q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                         new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 },
                         new Facts::Firm { Id = 3, OrganizationUnitId = 1 });

            var target = Mock.Of<IQuery>(
                q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1 },
                                                                               new CI::Firm { Id = 2 },
                                                                               new CI::Firm { Id = 3 }) &&
                     q.For<CI::FirmBalance>() == Inquire(new CI::FirmBalance { FirmId = 2, Balance = 123 },
                                                         new CI::FirmBalance { FirmId = 3, Balance = 123 }));

            Transformation.Create(source.Object, target)
                          .Transform(Aggregate.Recalculate<CI::Firm>(1),
                                     Aggregate.Recalculate<CI::Firm>(2),
                                     Aggregate.Recalculate<CI::Firm>(3))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 2, ClientId = 2 }))))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 3 }))))
                          .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 1, Balance = 123 }))))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 2, Balance = 456 }))))
                          .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 3, Balance = 123 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingCategory()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For<Facts::Category>(),
                         new Facts::Category { Id = 1, Level = 3 },
                         new Facts::Category { Id = 2, Level = 3 })
                  .Setup(q => q.For<Facts::CategoryOrganizationUnit>(),
                         new Facts::CategoryOrganizationUnit { CategoryId = 1, OrganizationUnitId = 1 },
                         new Facts::CategoryOrganizationUnit { CategoryId = 2, OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::FirmAddress>(),
                         new Facts::FirmAddress { Id = 1, FirmId = 1 },
                         new Facts::FirmAddress { Id = 2, FirmId = 2 })
                  .Setup(q => q.For<Facts::CategoryFirmAddress>(),
                         new Facts::CategoryFirmAddress { FirmAddressId = 1, CategoryId = 1 },
                         new Facts::CategoryFirmAddress { FirmAddressId = 2, CategoryId = 2 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, OrganizationUnitId = 1 },
                         new Facts::Firm { Id = 2, OrganizationUnitId = 1 },
                         new Facts::Firm { Id = 3, OrganizationUnitId = 1 });

            var target = Mock.Of<IQuery>(
                q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1 },
                                                                               new CI::Firm { Id = 2 },
                                                                               new CI::Firm { Id = 3 }) &&
                     q.For<CI::FirmCategory>() == Inquire(new CI::FirmCategory { FirmId = 2, CategoryId = 1 },
                                                          new CI::FirmCategory { FirmId = 3, CategoryId = 1 }));

            Transformation.Create(source.Object, target)
                          .Transform(Aggregate.Recalculate<CI::Firm>(1),
                                     Aggregate.Recalculate<CI::Firm>(2),
                                     Aggregate.Recalculate<CI::Firm>(3))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1, AddressCount = 1 }))))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 2, AddressCount = 1 }))))
                          .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 3 }))))
                          .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }))))
                          .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 2, CategoryId = 2 }))))
                          .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 2, CategoryId = 1 }))))
                          .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 3, CategoryId = 1 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingClient()
        {
            var source = new Mock<IQuery>();
            source.Setup(q => q.For<Facts::Category>(),
                         new Facts::Category { Id = 1, Level = 3 })
                  .Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::Client>(),
                         new Facts::Client { Id = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            var target = Mock.Of<IQuery>(
                q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1 }) &&
                     q.For(It.IsAny<FindSpecification<CI::Client>>()) == Inquire(new CI::Client { Id = 1 }));

            Transformation.Create(source.Object, target)
                .Transform(Aggregate.Recalculate<CI::Firm>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldDestroyFirm()
        {
            var source = Mock.Of<IQuery>();
            var target = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Firm>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingBalance()
        {
            var source = Mock.Of<IQuery>();
            var target = Mock.Of<IQuery>(
                q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1 }) &&
                     q.For<CI::FirmBalance>() == Inquire(new CI::FirmBalance { FirmId = 1, Balance = 123 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Firm>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 1, Balance = 123 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingCategory()
        {
            var source = Mock.Of<IQuery>();
            var target = Mock.Of<IQuery>(
                q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1 }) &&
                     q.For<CI::FirmCategory>() == Inquire(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Firm>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingClient()
        {
            var source = Mock.Of<IQuery>();
            var target = Mock.Of<IQuery>(
                q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1, ClientId = 1 }) &&
                     q.For(It.IsAny<FindSpecification<CI::Client>>()) == Inquire(new CI::Client { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Firm>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldInitializeProject()
        {
            var source = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Facts::Project>>()) == Inquire(new Facts::Project { Id = 1 }));
            var target = Mock.Of<IQuery>();

            Transformation.Create(source, target)
                .Transform(Aggregate.Initialize<CI::Project>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Project { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateProject()
        {
            var source = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Facts::Project>>()) == Inquire(new Facts::Project { Id = 1, Name = "new name" }));
            var target = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Project>>()) == Inquire(new CI::Project { Id = 1, Name = "old name" }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Recalculate<CI::Project>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Project { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyProject()
        {
            var source = Mock.Of<IQuery>();
            var target = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Project>>()) == Inquire(new CI::Project { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Project>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Project { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeTerritory()
        {
            var source = Mock.Of<IQuery>(
                q => q.For<Facts::Project>() == Inquire(new Facts::Project { Id = 1, OrganizationUnitId = 1 }) &&
                     q.For(It.IsAny<FindSpecification<Facts::Territory>>()) == Inquire(new Facts::Territory { Id = 1, OrganizationUnitId = 1 }));
            var target = Mock.Of<IQuery>();

            Transformation.Create(source, target)
                .Transform(Aggregate.Initialize<CI::Territory>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Territory { Id = 1, ProjectId = 1}))));
        }

        [Test]
        public void ShouldRecalculateTerritory()
        {
            var source = Mock.Of<IQuery>(
                q => q.For<Facts::Project>() == Inquire(new Facts::Project { Id = 1, OrganizationUnitId = 1 }) &&
                     q.For(It.IsAny<FindSpecification<Facts::Territory>>()) == Inquire(new Facts::Territory { Id = 1, OrganizationUnitId = 1, Name = "new name" }));
            var target = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Territory>>()) == Inquire(new CI::Territory { Id = 1, Name = "old name" }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Recalculate<CI::Territory>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Territory { Id = 1, ProjectId = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyTerritory()
        {
            var source = Mock.Of<IQuery>();
            var target = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Territory>>()) == Inquire(new CI::Territory { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Territory>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Territory { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeCategoryGroup()
        {
            var source = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Facts::CategoryGroup>>()) == Inquire(new Facts::CategoryGroup { Id = 1 }));
            var target = Mock.Of<IQuery>();

            Transformation.Create(source, target)
                .Transform(Aggregate.Initialize<CI::CategoryGroup>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::CategoryGroup { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateCategoryGroup()
        {
            var source = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Facts::CategoryGroup>>()) == Inquire(new Facts::CategoryGroup { Id = 1, Name = "new name" }));
            var target = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::CategoryGroup>>()) == Inquire(new CI::CategoryGroup { Id = 1, Name = "old name" }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Recalculate<CI::CategoryGroup>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::CategoryGroup { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyCategoryGroup()
        {
            var source = Mock.Of<IQuery>();
            var target = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::CategoryGroup>>()) == Inquire(new CI::CategoryGroup { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::CategoryGroup>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::CategoryGroup { Id = 1 }))));
        }

        #region Transformation

        private class Transformation
        {
            private readonly CustomerIntelligenceTransformation _transformation;
            private readonly Mock<IDataMapper> _mapper;

            private Transformation(IQuery source, IQuery target)
            {
                _mapper = new Mock<IDataMapper>();
                _transformation = new CustomerIntelligenceTransformation(source, target, _mapper.Object, Mock.Of<ITransactionManager>());
            }

            public static Transformation Create(IQuery source = null, IQuery target = null)
            {
                return new Transformation(source ?? new Mock<IQuery>().Object, target ?? new Mock<IQuery>().Object);
            }

            public Transformation Transform(params AggregateOperation[] operations)
            {
                _transformation.Transform(operations);
                return this;
            }

            public Transformation Verify(Expression<Action<IDataMapper>> action, Func<Times> times = null, string failMessage = null)
            {
                _mapper.Verify(action, times ?? Times.AtLeastOnce, failMessage);
                return this;
            }
        }

        #endregion
    }
}