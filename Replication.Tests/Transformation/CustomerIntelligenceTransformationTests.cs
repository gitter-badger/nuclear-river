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
                ctx.Contacts == Inquire(new CI::Contact { Id = 1, ClientId = 1 }));

            Transformation.Create(source)
                .Transform(Aggregate.Initialize<CI::Client>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Contact { Id = 1 }, x => x.Id))), Times.Never);
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
                ctx.Contacts == Inquire(
                    new CI::Contact { Id = 1, ClientId = 1 },
                    new CI::Contact { Id = 2, ClientId = 2 }
                    ));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Clients == Inquire(
                    new CI::Client { Id = 1 },
                    new CI::Client { Id = 2 },
                    new CI::Client { Id = 3 }
                    ) &&
                ctx.Contacts == Inquire(
                    new CI::Contact { Id = 2, ClientId = 2 },
                    new CI::Contact { Id = 3, ClientId = 3 }
                    ));

            Transformation.Create(source, target)
                .Transform(
                    Aggregate.Recalculate<CI::Client>(1),
                    Aggregate.Recalculate<CI::Client>(2),
                    Aggregate.Recalculate<CI::Client>(3))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 2 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 3 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Contact { Id = 1, ClientId = 1 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Contact { Id = 2, ClientId = 2 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Contact { Id = 3, ClientId = 3 }))));
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
                ctx.Contacts == Inquire(new CI::Contact { Id = 1, ClientId = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Client>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Contact { Id = 1, ClientId = 1 }))));
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
        public void ShouldInitializeFirmHavingCategoryGroup()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx => 
                ctx.Firms == Inquire(new CI::Firm { Id = 1 }) &&
                ctx.FirmCategoryGroups == Inquire(new CI::FirmCategoryGroup { FirmId = 1, CategoryGroupId = 1 }));

            Transformation.Create(source)
                .Transform(Aggregate.Initialize<CI::Firm>(1))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategoryGroup { FirmId = 1, CategoryGroupId = 1 }))));
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
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 2, Balance = 456 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 2, Balance = 123 }))))
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
        public void ShouldRecalculateFirmHavingCategoryGroup()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(
                    new CI::Firm { Id = 1 },
                    new CI::Firm { Id = 2 },
                    new CI::Firm { Id = 3 }
                    ) &&
                ctx.FirmCategoryGroups == Inquire(
                    new CI::FirmCategoryGroup { FirmId = 1, CategoryGroupId = 1 },
                    new CI::FirmCategoryGroup { FirmId = 2, CategoryGroupId = 2 }
                    ));
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(
                    new CI::Firm { Id = 1 },
                    new CI::Firm { Id = 2 },
                    new CI::Firm { Id = 3 }
                    ) &&
                ctx.FirmCategoryGroups == Inquire(
                    new CI::FirmCategoryGroup { FirmId = 2, CategoryGroupId = 1 },
                    new CI::FirmCategoryGroup { FirmId = 3, CategoryGroupId = 1 }
                    ));

            Transformation.Create(source, target)
                .Transform(
                    Aggregate.Recalculate<CI::Firm>(1),
                    Aggregate.Recalculate<CI::Firm>(2),
                    Aggregate.Recalculate<CI::Firm>(3))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 2 }))))
                .Verify(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 3 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategoryGroup { FirmId = 1, CategoryGroupId = 1 }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategoryGroup { FirmId = 2, CategoryGroupId = 2 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategoryGroup { FirmId = 2, CategoryGroupId = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategoryGroup { FirmId = 3, CategoryGroupId = 1 }))));
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
        public void ShouldDestroyFirmHavingCategoryGroup()
        {
            var source = Mock.Of<ICustomerIntelligenceContext>();
            var target = Mock.Of<ICustomerIntelligenceContext>(ctx =>
                ctx.Firms == Inquire(new CI::Firm { Id = 1 }) &&
                ctx.FirmCategoryGroups == Inquire(new CI::FirmCategoryGroup { FirmId = 1, CategoryGroupId = 1 }));

            Transformation.Create(source, target)
                .Transform(Aggregate.Destroy<CI::Firm>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategoryGroup { FirmId = 1, CategoryGroupId = 1 }))));
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

        #region Transformation

        private class Transformation
        {
            private readonly CustomerIntelligenceTransformation _transformation;
            private readonly Mock<IDataMapper> _mapper;

            private Transformation(ICustomerIntelligenceContext source, ICustomerIntelligenceContext target)
            {
                _mapper = new Mock<IDataMapper>();
                _transformation = new CustomerIntelligenceTransformation(source, target, _mapper.Object);
            }

            public static Transformation Create(IDataContext source = null, IDataContext target = null)
            {
                return Create(
                    new FactsContext(source ?? new Mock<IDataContext>().Object),
                    new CustomerIntelligenceContext(target ?? new Mock<IDataContext>().Object));
            }

            public static Transformation Create(IFactsContext source = null, ICustomerIntelligenceContext target = null)
            {
                return Create(new CustomerIntelligenceTransformationContext(source ?? new Mock<IFactsContext>().Object), target);
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