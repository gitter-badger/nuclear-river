using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Moq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;
using NuClear.Storage.Writings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal class CustomerIntelligenceTransformationTests : TransformationFixtureBase
    {
        [Test]
        public void ShouldInitializeClient()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For(It.IsAny<FindSpecification<Facts::Client>>()),
                         new Facts::Client { Id = 1 });

            Transformation.Create(query.Object)
                          .Transform(Aggregate.Initialize<CI::Client>(1))
                          .Verify<CI::Client>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Client { Id = 1 } }))));
        }

        [Test]
        public void ShouldInitializeClientHavingContact()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For(It.IsAny<FindSpecification<Facts::Client>>()),
                         new Facts::Client { Id = 1 })
                  .Setup(q => q.For<Facts::Contact>(),
                         new Facts::Contact { Id = 1, ClientId = 1 });

            Transformation.Create(query.Object)
                          .Transform(Aggregate.Initialize<CI::Client>(1))
                          .Verify<CI::Client>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Client { Id = 1 } }))))
                          .Verify<CI::ClientContact>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::ClientContact { ClientId = 1, ContactId = 1 } }))));
        }

        [Test]
        public void ShouldRecalculateClient()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For(It.IsAny<FindSpecification<Facts::Client>>()),
                        new Facts::Client { Id = 1, Name = "new name" });
            // CI
            query.Setup(q => q.For(It.IsAny<FindSpecification<CI::Client>>()),
                        new CI::Client { Id = 1, Name = "old name" });

            Transformation.Create(query.Object)
                .Transform(Aggregate.Recalculate<CI::Client>(1))
                .Verify<CI::Client>(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldRecalculateClientHavingContact()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For(It.IsAny<FindSpecification<Facts::Client>>()),
                         new Facts::Client { Id = 1 },
                         new Facts::Client { Id = 2 },
                         new Facts::Client { Id = 3 })
                  .Setup(q => q.For<Facts::Contact>(),
                         new Facts::Contact { Id = 1, ClientId = 1 },
                         new Facts::Contact { Id = 2, ClientId = 2 });

            // CI
            query.Setup(q => q.For(It.IsAny<FindSpecification<CI::Client>>()),
                         new CI::Client { Id = 1 },
                         new CI::Client { Id = 2 },
                         new CI::Client { Id = 3 })
                  .Setup(q => q.For<CI::ClientContact>(),
                         new CI::ClientContact { ClientId = 2, ContactId = 2 },
                         new CI::ClientContact { ClientId = 3, ContactId = 3 });

            Transformation.Create(query.Object)
                          .Transform(Aggregate.Recalculate<CI::Client>(1),
                                     Aggregate.Recalculate<CI::Client>(2),
                                     Aggregate.Recalculate<CI::Client>(3))
                          .Verify<CI::Client>(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                          .Verify<CI::Client>(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 2 }))))
                          .Verify<CI::Client>(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 3 }))))
                          .Verify<CI::ClientContact>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::ClientContact { ClientId = 1, ContactId = 1 } }))))
                          .Verify<CI::ClientContact>(m => m.Update(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 2, ContactId = 2 }))))
                          .Verify<CI::ClientContact>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::ClientContact { ClientId = 3, ContactId = 3 } }))));
        }

        [Test]
        public void ShouldDestroyClient()
        {
            // CI
            var query = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Client>>()) == Inquire(new CI::Client { Id = 1 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Destroy<CI::Client>(1))
                          .Verify<CI::Client>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Client { Id = 1 } }))));
        }

        [Test]
        public void ShouldDestroyClientHavingContact()
        {
            // CI
            var query = Mock.Of<IQuery>(q =>
                q.For(It.IsAny<FindSpecification<CI::Client>>()) == Inquire(new CI::Client { Id = 1 }) &&
                q.For<CI::ClientContact>() == Inquire(new CI::ClientContact { ClientId = 1, ContactId = 1 }));

            Transformation.Create(query)
                .Transform(Aggregate.Destroy<CI::Client>(1))
                .Verify<CI::Client>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Client { Id = 1 }}))))
                .Verify<CI::ClientContact>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::ClientContact { ClientId = 1, ContactId = 1 }}))));
        }

        [Test]
        public void ShouldInitializeFirm()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(query.Object)
                          .Transform(Aggregate.Initialize<CI::Firm>(1))
                          .Verify<CI::Firm>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Firm { Id = 1 } }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingBalance()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For<Facts::Project>(),
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

            Transformation.Create(query.Object)
                          .Transform(Aggregate.Initialize<CI::Firm>(1))
                          .Verify<CI::Firm>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Firm { Id = 1, ClientId = 1 } }))))
                          .Verify<CI::FirmBalance>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::FirmBalance { FirmId = 1, Balance = 123.45m } }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingCategory()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For<Facts::Category>(),
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

            Transformation.Create(query.Object)
                          .Transform(Aggregate.Initialize<CI::Firm>(1))
                          .Verify<CI::Firm>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Firm { Id = 1, AddressCount = 1 } }))))
                          .Verify<CI::FirmCategory>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::FirmCategory { FirmId = 1, CategoryId = 1 } }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingClient()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For<Facts::Category>(),
                         new Facts::Category { Id = 1, Level = 3 })
                  .Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For<Facts::Client>(),
                         new Facts::Client { Id = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(query.Object)
                          .Transform(Aggregate.Initialize<CI::Firm>(1))
                          .Verify<CI::Firm>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Firm { Id = 1, ClientId = 1 } }))))
                          .Verify<CI::Client>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Client { Id = 1 } }))), Times.Never);
        }

        [Test]
        public void ShouldRecalculateFirm()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For<Facts::Project>(),
                         new Facts::Project { OrganizationUnitId = 1 })
                  .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                         new Facts::Firm { Id = 1, OrganizationUnitId = 1 });

            // CI
            query.Setup(q => q.For(It.IsAny<FindSpecification<CI::Firm>>()),
                        new CI::Firm { Id = 1 });

            Transformation.Create(query.Object)
                .Transform(Aggregate.Recalculate<CI::Firm>(1))
                .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingBalance()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For<Facts::Project>(),
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

            // CI
            query.Setup(q => q.For(It.IsAny<FindSpecification<CI::Firm>>()),
                        new CI::Firm { Id = 1 },
                        new CI::Firm { Id = 2 },
                        new CI::Firm { Id = 3 })
                 .Setup(q => q.For<CI::FirmBalance>(),
                        new CI::FirmBalance { FirmId = 2, Balance = 123 },
                        new CI::FirmBalance { FirmId = 3, Balance = 123 });

            Transformation.Create(query.Object)
                          .Transform(Aggregate.Recalculate<CI::Firm>(1),
                                     Aggregate.Recalculate<CI::Firm>(2),
                                     Aggregate.Recalculate<CI::Firm>(3))
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 2, ClientId = 2 }))))
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 3 }))))
                          .Verify<CI::FirmBalance>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::FirmBalance { FirmId = 1, Balance = 123 } }))))
                          .Verify<CI::FirmBalance>(m => m.Update(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 2, Balance = 456 }))))
                          .Verify<CI::FirmBalance>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::FirmBalance { FirmId = 3, Balance = 123 } }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingCategory()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For<Facts::Category>(),
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

            // CI
            query.Setup(q => q.For(It.IsAny<FindSpecification<CI::Firm>>()),
                        new CI::Firm { Id = 1 },
                        new CI::Firm { Id = 2 },
                        new CI::Firm { Id = 3 })
                 .Setup(q => q.For<CI::FirmCategory>(),
                        new CI::FirmCategory { FirmId = 2, CategoryId = 1 },
                        new CI::FirmCategory { FirmId = 3, CategoryId = 1 });

            Transformation.Create(query.Object)
                          .Transform(Aggregate.Recalculate<CI::Firm>(1),
                                     Aggregate.Recalculate<CI::Firm>(2),
                                     Aggregate.Recalculate<CI::Firm>(3))
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1, AddressCount = 1 }))))
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 2, AddressCount = 1 }))))
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 3 }))))
                          .Verify<CI::FirmCategory>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[]
                                                                                                    {
                                                                                                        new CI::FirmCategory { FirmId = 1, CategoryId = 1 },
                                                                                                        new CI::FirmCategory { FirmId = 2, CategoryId = 2 }
                                                                                                    }))))
                          .Verify<CI::FirmCategory>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[]
                                                                                                       {
                                                                                                           new CI::FirmCategory { FirmId = 2, CategoryId = 1 },
                                                                                                           new CI::FirmCategory { FirmId = 3, CategoryId = 1 }
                                                                                                       }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingClient()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For<Facts::Category>(),
                        new Facts::Category { Id = 1, Level = 3 })
                 .Setup(q => q.For<Facts::Project>(),
                        new Facts::Project { OrganizationUnitId = 1 })
                 .Setup(q => q.For<Facts::Client>(),
                        new Facts::Client { Id = 1 })
                 .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Firm>>()),
                        new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });
            // CI
            query.Setup(q => q.For(It.IsAny<FindSpecification<CI::Firm>>()),
                        new CI::Firm { Id = 1 })
                 .Setup(q => q.For(It.IsAny<FindSpecification<CI::Client>>()),
                        new CI::Client { Id = 1 });

            Transformation.Create(query.Object)
                .Transform(Aggregate.Recalculate<CI::Firm>(1))
                .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                .Verify<CI::Client>(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldDestroyFirm()
        {
            // CI
            var query = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Destroy<CI::Firm>(1))
                          .Verify<CI::Firm>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Firm { Id = 1 } }))));
        }

        [Test]
        public void ShouldDestroyFirmUsingSqlLite()
        {
            CustomerIntelligenceDb.Has(new CI::Firm { Id = 1 });

            Transformation.Create(Query)
                          .Transform(Aggregate.Destroy<CI::Firm>(1))
                          .Verify<CI::Firm>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Firm { Id = 1 } }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingBalance()
        {
            // CI
            var query = Mock.Of<IQuery>(
                q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1 }) &&
                     q.For<CI::FirmBalance>() == Inquire(new CI::FirmBalance { FirmId = 1, Balance = 123 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Destroy<CI::Firm>(1))
                          .Verify<CI::Firm>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Firm { Id = 1 } }))))
                          .Verify<CI::FirmBalance>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::FirmBalance { FirmId = 1, Balance = 123 } }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingCategory()
        {
            // CI
            var query = Mock.Of<IQuery>(
                q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1 }) &&
                     q.For<CI::FirmCategory>() == Inquire(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Destroy<CI::Firm>(1))
                          .Verify<CI::Firm>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Firm { Id = 1 } }))))
                          .Verify<CI::FirmCategory>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::FirmCategory { FirmId = 1, CategoryId = 1 } }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingClient()
        {
            // CI
            var query = Mock.Of<IQuery>(
                q => q.For(It.IsAny<FindSpecification<CI::Firm>>()) == Inquire(new CI::Firm { Id = 1, ClientId = 1 }) &&
                     q.For(It.IsAny<FindSpecification<CI::Client>>()) == Inquire(new CI::Client { Id = 1 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Destroy<CI::Firm>(1))
                          .Verify<CI::Firm>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Firm { Id = 1, ClientId = 1 } }))))
                          .Verify<CI::Client>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Client { Id = 1 } }))), Times.Never);
        }

        [Test]
        public void ShouldInitializeProject()
        {
            // Facts
            var query = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Facts::Project>>()) == Inquire(new Facts::Project { Id = 1 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Initialize<CI::Project>(1))
                          .Verify<CI::Project>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Project { Id = 1 } }))));
        }

        [Test]
        public void ShouldRecalculateProject()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For(It.IsAny<FindSpecification<Facts::Project>>()),
                        new Facts::Project { Id = 1, Name = "new name" });

            // CI
            query.Setup(q => q.For(It.IsAny<FindSpecification<CI::Project>>()),
                        new CI::Project { Id = 1, Name = "old name" });

            Transformation.Create(query.Object)
                .Transform(Aggregate.Recalculate<CI::Project>(1))
                .Verify<CI::Project>(m => m.Update(It.Is(Predicate.Match(new CI::Project { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyProject()
        {
            // CI
            var query = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Project>>()) == Inquire(new CI::Project { Id = 1 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Destroy<CI::Project>(1))
                          .Verify<CI::Project>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Project { Id = 1 } }))));
        }

        [Test]
        public void ShouldInitializeTerritory()
        {
            // Facts
            var query = Mock.Of<IQuery>(
                q => q.For<Facts::Project>() == Inquire(new Facts::Project { Id = 1, OrganizationUnitId = 1 }) &&
                     q.For(It.IsAny<FindSpecification<Facts::Territory>>()) == Inquire(new Facts::Territory { Id = 1, OrganizationUnitId = 1 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Initialize<CI::Territory>(1))
                          .Verify<CI::Territory>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Territory { Id = 1, ProjectId = 1 } }))));
        }

        [Test]
        public void ShouldRecalculateTerritory()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For<Facts::Project>(), (new Facts::Project { Id = 1, OrganizationUnitId = 1 }))
                .Setup(q => q.For(It.IsAny<FindSpecification<Facts::Territory>>()), new Facts::Territory { Id = 1, OrganizationUnitId = 1, Name = "new name" });

            // CI
            query.Setup(q => q.For(It.IsAny<FindSpecification<CI::Territory>>()), new CI::Territory { Id = 1, Name = "old name" });

            Transformation.Create(query.Object)
                .Transform(Aggregate.Recalculate<CI::Territory>(1))
                .Verify<CI::Territory>(m => m.Update(It.Is(Predicate.Match(new CI::Territory { Id = 1, ProjectId = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyTerritory()
        {
            // CI
            var query = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::Territory>>()) == Inquire(new CI::Territory { Id = 1 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Destroy<CI::Territory>(1))
                          .Verify<CI::Territory>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::Territory { Id = 1 } }))));
        }

        [Test]
        public void ShouldInitializeCategoryGroup()
        {
            // Facts
            var query = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<Facts::CategoryGroup>>()) == Inquire(new Facts::CategoryGroup { Id = 1 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Initialize<CI::CategoryGroup>(1))
                          .Verify<CI::CategoryGroup>(m => m.AddRange(It.Is(Predicate.SequentialMatch(new[] { new CI::CategoryGroup { Id = 1 } }))));
        }

        [Test]
        public void ShouldRecalculateCategoryGroup()
        {
            var query = new Mock<IQuery>();

            // Facts
            query.Setup(q => q.For(It.IsAny<FindSpecification<Facts::CategoryGroup>>()), new Facts::CategoryGroup { Id = 1, Name = "new name" });

            // CI
            query.Setup(q => q.For(It.IsAny<FindSpecification<CI::CategoryGroup>>()), new CI::CategoryGroup { Id = 1, Name = "old name" });

            Transformation.Create(query.Object)
                .Transform(Aggregate.Recalculate<CI::CategoryGroup>(1))
                .Verify<CI::CategoryGroup>(m => m.Update(It.Is(Predicate.Match(new CI::CategoryGroup { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyCategoryGroup()
        {
            // CI
            var query = Mock.Of<IQuery>(q => q.For(It.IsAny<FindSpecification<CI::CategoryGroup>>()) == Inquire(new CI::CategoryGroup { Id = 1 }));

            Transformation.Create(query)
                          .Transform(Aggregate.Destroy<CI::CategoryGroup>(1))
                          .Verify<CI::CategoryGroup>(m => m.DeleteRange(It.Is(Predicate.SequentialMatch(new[] { new CI::CategoryGroup { Id = 1 } }))));
        }

        #region Transformation

        private class Transformation
        {
            private static readonly IDictionary<Type, IRepository> RepositoriesToVerify = new Dictionary<Type, IRepository>();
            private readonly CustomerIntelligenceTransformation _transformation;
            
            private Transformation(IQuery source)
            {
                RepositoriesToVerify.Clear();
                _transformation = new CustomerIntelligenceTransformation(source, new VerifiableDataChangesApplierFactory(OnRepositoryCreated));
            }

            public static Transformation Create(IQuery source)
            {
                return new Transformation(source);
            }

            public Transformation Transform(params AggregateOperation[] operations)
            {
                _transformation.Transform(operations);
                return this;
            }

            public Transformation Verify<T>(Expression<Action<IRepository<T>>> action, Func<Times> times = null, string failMessage = null) where T : class
            {
                if (times != null && times == Times.Never)
                {
                    if (RepositoriesToVerify.ContainsKey(typeof(T)))
                    {
                        throw new AssertionException(failMessage);
                    }

                    return this;
                }

                var repository = (IRepository<T>)RepositoriesToVerify[typeof(T)];
                Mock.Get(repository).Verify(action, times ?? Times.AtLeastOnce, failMessage);
                return this;
            }

            private static void OnRepositoryCreated(Type type, IRepository repository)
            {
                RepositoriesToVerify.Add(type, repository);
            }
        }

        #endregion
    }
}