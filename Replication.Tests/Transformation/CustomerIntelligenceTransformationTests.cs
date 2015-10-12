using System;
using System.Linq.Expressions;

using LinqToDB;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;

    [TestFixture]
    internal class CustomerIntelligenceTransformationTests : BaseTransformationFixture
    {
        [Test]
        public void ShouldInitializeClient()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => 
                ctx.Clients == Inquire(new CI::Client { Id = 1 }));

            Transformation.Create(source)
                .Transform(Aggregate.Initialize<CI::Client>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Client { Id = 1 }))));
        }

       [Test]
        public void ShouldInitializeClientHavingContact()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Clients == Inquire(new CI::Client { Id = 1 }) &&
                ctx.ClientContacts == Inquire(new CI::ClientContact { ClientId = 1, ContactId = 1 }));

            Transformation.Create(source)
                .Transform(Aggregate.Initialize<CI::Client>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 1, ContactId = 1 }))));
        }

        [Test]
        public void ShouldRecalculateClient()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Clients == Inquire(new CI::Client { Id = 1, Name = "new name" }));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Clients == Inquire(new CI::Client { Id = 1, Name = "old name" }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Recalculate<CI::Client>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldRecalculateClientHavingContact()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Clients == Inquire(
                    new CI::Client { Id = 1 },
                    new CI::Client { Id = 2 },
                    new CI::Client { Id = 3 }
                    ) &&
                ctx.ClientContacts == Inquire(
                    new CI::ClientContact { ClientId = 1, ContactId = 1 },
                    new CI::ClientContact { ClientId = 2, ContactId = 2 }
                    ));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Clients == Inquire(
                    new CI::Client { Id = 1 },
                    new CI::Client { Id = 2 },
                    new CI::Client { Id = 3 }
                    ) &&
                ctx.ClientContacts == Inquire(
                    new CI::ClientContact { ClientId = 2, ContactId = 2 },
                    new CI::ClientContact { ClientId = 3, ContactId = 3 }
                    ));

            Transformation.Create(source, target)
                .Transform(
                    Aggregate.Recalculate<CI::Client>(1),
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
            var source = Mock.Of<ICustomerIntelligenceContext>();
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Clients == Inquire(new CI::Client { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Client>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Client { Id = 1 }))));
        }

        [Test]
        public void ShouldDestroyClientHavingContact()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>();
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Clients == Inquire(new CI::Client { Id = 1 }) &&
                ctx.ClientContacts == Inquire(new CI::ClientContact { ClientId = 1, ContactId = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Client>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 1, ContactId = 1 }))));
        }

        [Test]
        public void ShouldInitializeFirm()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Firms == Inquire(new CI::Firm { Id = 1 }));

            Transformation.Create(source)
                .Transform(Aggregate.Initialize<CI::Firm>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))));
        }
        
        [Test]
        public void ShouldInitializeFirmHavingBalance()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => 
                ctx.Firms == Inquire(new CI::Firm { Id = 1 }) &&
                ctx.FirmBalances == Inquire(new CI::FirmBalance { FirmId = 1, Balance = 123.45m }));

            Transformation.Create(source)
                .Transform(Aggregate.Initialize<CI::Firm>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 1, Balance = 123.45m }))));
        }
        
        [Test]
        public void ShouldInitializeFirmHavingCategory()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => 
                ctx.Firms == Inquire(new CI::Firm { Id = 1 }) &&
                ctx.FirmCategories == Inquire(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }));

            Transformation.Create(source)
                .Transform(Aggregate.Initialize<CI::Firm>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }))));
        }
        
        [Test]
        public void ShouldInitializeFirmHavingClient()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => 
                ctx.Firms == Inquire(new CI::Firm { Id = 1, ClientId = 1 }) &&
                ctx.Clients == Inquire(new CI::Client { Id = 1 }));

            Transformation.Create(source)
                .Transform(Aggregate.Initialize<CI::Firm>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldRecalculateFirm()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Firms == Inquire(new CI::Firm { Id = 1 }));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Firms == Inquire(new CI::Firm { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Recalculate<CI::Firm>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingBalance()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(
                    new CI::Firm { Id = 1 },
                    new CI::Firm { Id = 2 },
                    new CI::Firm { Id = 3 }
                    ) &&
                ctx.FirmBalances == Inquire(
                    new CI::FirmBalance { FirmId = 1, Balance = 123 },
                    new CI::FirmBalance { FirmId = 2, Balance = 456 }
                    ));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(
                    new CI::Firm { Id = 1 },
                    new CI::Firm { Id = 2 },
                    new CI::Firm { Id = 3 }
                    ) &&
                ctx.FirmBalances == Inquire(
                    new CI::FirmBalance { FirmId = 2, Balance = 123 },
                    new CI::FirmBalance { FirmId = 3, Balance = 123 }
                    ));

            Transformation.Create(source, target)
                .Transform(
                    Aggregate.Recalculate<CI::Firm>(1),
                    Aggregate.Recalculate<CI::Firm>(2),
                    Aggregate.Recalculate<CI::Firm>(3))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 2 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 3 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 1, Balance = 123 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 2, Balance = 456 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 3, Balance = 123 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingCategory()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(
                    new CI::Firm { Id = 1 },
                    new CI::Firm { Id = 2 },
                    new CI::Firm { Id = 3 }
                    ) &&
                ctx.FirmCategories == Inquire(
                    new CI::FirmCategory { FirmId = 1, CategoryId = 1 },
                    new CI::FirmCategory { FirmId = 2, CategoryId = 2 }
                    ));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(
                    new CI::Firm { Id = 1 },
                    new CI::Firm { Id = 2 },
                    new CI::Firm { Id = 3 }
                    ) &&
                ctx.FirmCategories == Inquire(
                    new CI::FirmCategory { FirmId = 2, CategoryId = 1 },
                    new CI::FirmCategory { FirmId = 3, CategoryId = 1 }
                    ));

            Transformation.Create(source, target)
                .Transform(
                    Aggregate.Recalculate<CI::Firm>(1),
                    Aggregate.Recalculate<CI::Firm>(2),
                    Aggregate.Recalculate<CI::Firm>(3))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 2 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 3 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 2, CategoryId = 2 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 2, CategoryId = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 3, CategoryId = 1 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingClient()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(new CI::Firm { Id = 1, ClientId = 1 }) &&
                ctx.Clients == Inquire(new CI::Client { Id = 1 }));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(new CI::Firm { Id = 1 }) &&
                ctx.Clients == Inquire(new CI::Client { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Recalculate<CI::Firm>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldRecalculateFirmHavingTerritory()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(new CI::Firm { Id = 1 }) &&
                ctx.FirmTerritories == Inquire(new CI::FirmTerritory { FirmId = 1, TerritoryId = 2 }));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(new CI::Firm { Id = 1 }) &&
                ctx.FirmTerritories == Inquire(new CI::FirmTerritory { FirmId = 1, TerritoryId = 3 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Recalculate<CI::Firm>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::FirmTerritory { FirmId = 1, TerritoryId = 2 }))));
        }

        [Test]
        public void ShouldDestroyFirm()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>();
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Firms == Inquire(new CI::Firm { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Firm>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingBalance()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>();
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(new CI::Firm { Id = 1 }) &&
                ctx.FirmBalances == Inquire(new CI::FirmBalance { FirmId = 1, Balance = 123 }));

            Transformation.Create(source, target)
                .Transform(
                    Aggregate.Destroy<CI::Firm>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 1, Balance = 123 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingCategory()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>();
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(new CI::Firm { Id = 1 }) &&
                ctx.FirmCategories == Inquire(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Firm>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingClient()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>();
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(new CI::Firm { Id = 1, ClientId = 1 }) &&
                ctx.Clients == Inquire(new CI::Client { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Firm>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldInitializeProject()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Projects == Inquire(new CI::Project { Id = 1 }));
            var target = Mock.Of<ICustomerIntelligenceContext>();

            Transformation.Create(source, target)
                .Transform(Aggregate.Initialize<CI::Project>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Project { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateProject()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Projects == Inquire(new CI::Project { Id = 1, Name = "new name" }));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Projects == Inquire(new CI::Project { Id = 1, Name = "old name" }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Recalculate<CI::Project>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Project { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyProject()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>();
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Projects == Inquire(new CI::Project { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Project>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Project { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeTerritory()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Territories == Inquire(new CI::Territory { Id = 1 }));
            var target = Mock.Of<ICustomerIntelligenceContext>();

            Transformation.Create(source, target)
                .Transform(Aggregate.Initialize<CI::Territory>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Territory { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateTerritory()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Territories == Inquire(new CI::Territory { Id = 1, Name = "new name" }));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Territories == Inquire(new CI::Territory { Id = 1, Name = "old name" }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Recalculate<CI::Territory>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Territory { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyTerritory()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>();
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.Territories == Inquire(new CI::Territory { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Territory>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Territory { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeCategoryGroup()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.CategoryGroups == Inquire(new CI::CategoryGroup { Id = 1 }));
            var target = Mock.Of<ICustomerIntelligenceContext>();

            Transformation.Create(source, target)
                .Transform(Aggregate.Initialize<CI::CategoryGroup>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::CategoryGroup { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateCategoryGroup()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.CategoryGroups == Inquire(new CI::CategoryGroup { Id = 1, Name = "new name" }));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.CategoryGroups == Inquire(new CI::CategoryGroup { Id = 1, Name = "old name" }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Recalculate<CI::CategoryGroup>(1))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::CategoryGroup { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyCategoryGroup()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>();
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx => ctx.CategoryGroups == Inquire(new CI::CategoryGroup { Id = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::CategoryGroup>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::CategoryGroup { Id = 1 }))));
        }

        #region Transformation

        private class Transformation
        {
            private readonly CustomerIntelligenceTransformation _transformation;
            private readonly Mock<IDataMapper> _mapper;

            private Transformation(ICustomerIntelligenceContext source, ICustomerIntelligenceContext target)
            {
                _mapper = new Mock<IDataMapper>();
                _transformation = new CustomerIntelligenceTransformation(source, target, _mapper.Object, Mock.Of<ITransactionManager>());
            }

            public static Transformation Create(IDataContext source = null, IDataContext target = null)
            {
                return Create(
                    new ErmFactsContext(source ?? new Mock<IDataContext>().Object),
                    new CustomerIntelligenceContext(target ?? new Mock<IDataContext>().Object));
            }

            public static Transformation Create(IErmFactsContext source = null, ICustomerIntelligenceContext target = null)
            {
                return Create(new CustomerIntelligenceTransformationContext(source ?? new Mock<IErmFactsContext>().Object), target);
            }

            public static Transformation Create(ICustomerIntelligenceContext source = null, ICustomerIntelligenceContext target = null)
            {
                return new Transformation(source ?? new Mock<ICustomerIntelligenceContext>().Object, target ?? new Mock<ICustomerIntelligenceContext>().Object);
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