﻿using System;
using System.Collections;
using System.Linq.Expressions;

using LinqToDB;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;
using NuClear.AdvancedSearch.Replication.Tests.Data;
using NuClear.AdvancedSearch.Replication.Transforming;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Facts = CustomerIntelligence.Model.Facts;
    using CI = CustomerIntelligence.Model;

    /// <remarks>
    /// Executes in invariant culture to simplify expected result after the formatting.
    /// </remarks>>
    [TestFixture, SetCulture("")]
    internal class CustomerIntelligenceTransformationTests : BaseTransformationFixture
    {
        [Test]
        public void ShouldProcessFirmDisqualifiedDate()
        {
            var firmDate = new DateTimeOffset(2015, 1, 1, 12, 30, 0, new TimeSpan());
            var clientDate = new DateTimeOffset(2015, 2, 1, 12, 30, 0, new TimeSpan());

            var context = new Mock<IFactsContext>();
            context.SetupGet(x => x.Firms).Returns(Inquire(
                new Facts::Firm { Id = 1, LastDisqualifiedOn = firmDate },
                new Facts::Firm { Id = 2, LastDisqualifiedOn = firmDate , ClientId = 1}
                ));
            context.SetupGet(x => x.Clients).Returns(Inquire(
                new Facts::Client { Id = 1, LastDisqualifiedOn = clientDate }
                ));

            Transformation.Create(context.Object)
                .Transform(Operation.Create<CI::Firm>(1))
                .Transform(Operation.Create<CI::Firm>(2))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1, LastDisqualifiedOn = firmDate }, x => new { x.Id, x.LastDisqualifiedOn }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 2, LastDisqualifiedOn = clientDate }, x => new { x.Id, x.LastDisqualifiedOn }))));
        }

        [Test]
        public void ShouldProcessFirmDistributionDate()
        {
            var date = new DateTimeOffset(2015, 2, 1, 12, 30, 0, new TimeSpan());

            var context = new Mock<IFactsContext>();
            context.SetupGet(x => x.Firms).Returns(Inquire(
                new Facts::Firm { Id = 1 },
                new Facts::Firm { Id = 2 }
                ));
            context.SetupGet(x => x.Orders).Returns(Inquire(
                new Facts::Order { Id = 1, EndDistributionDateFact = date, FirmId = 2 }
                ));

            Transformation.Create(context.Object)
                .Transform(Operation.Create<CI::Firm>(1))
                .Transform(Operation.Create<CI::Firm>(2))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1, LastDistributedOn = null }, x => new { x.Id, x.LastDistributedOn }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 2, LastDistributedOn = date }, x => new { x.Id, x.LastDistributedOn }))));
        }

        [Test]
        public void ShouldProcessFirmHasPhoneFlag()
        {
            var context = new Mock<IFactsContext>();
            context.SetupGet(x => x.Firms).Returns(Inquire(
                new Facts::Firm { Id = 1, Name = "no phone" },
                new Facts::Firm { Id = 2, Name = "has phone in client", ClientId = 1},
                new Facts::Firm { Id = 3, Name = "has phone in client contact", ClientId = 2}
                ));
            context.SetupGet(x => x.Clients).Returns(Inquire(
                new Facts::Client { Id = 1, HasPhone = true },
                new Facts::Client { Id = 2 }
                ));
            context.SetupGet(x => x.Contacts).Returns(Inquire(
                new Facts::Contact { Id = 1, HasPhone = true, ClientId = 2 }
                ));

            Transformation.Create(context.Object)
                .Transform(
                    Operation.Create<CI::Firm>(1), 
                    Operation.Create<CI::Firm>(2), 
                    Operation.Create<CI::Firm>(3))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1, HasPhone = false }, x => new { x.Id, x.HasPhone }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 2, HasPhone = true }, x => new { x.Id, x.HasPhone }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 3, HasPhone = true }, x => new { x.Id, x.HasPhone }))));
        }

        [Test]
        public void ShouldProcessFirmHasWebsiteFlag()
        {
            var context = new Mock<IFactsContext>();
            context.SetupGet(x => x.Firms).Returns(Inquire(
                new Facts::Firm { Id = 1, Name = "no website" },
                new Facts::Firm { Id = 2, Name = "has website in client", ClientId = 1 },
                new Facts::Firm { Id = 3, Name = "has website in client contact", ClientId = 2 }
                ));
            context.SetupGet(x => x.Clients).Returns(Inquire(
                new Facts::Client { Id = 1, HasWebsite = true },
                new Facts::Client { Id = 2 }
                ));
            context.SetupGet(x => x.Contacts).Returns(Inquire(
                new Facts::Contact { Id = 1, HasWebsite = true, ClientId = 2 }
                ));

            Transformation.Create(context.Object)
                .Transform(
                    Operation.Create<CI::Firm>(1),
                    Operation.Create<CI::Firm>(2),
                    Operation.Create<CI::Firm>(3))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1, HasWebsite = false }, x => new { x.Id, x.HasWebsite }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 2, HasWebsite = true }, x => new { x.Id, x.HasWebsite }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 3, HasWebsite = true }, x => new { x.Id, x.HasWebsite }))));
        }

        [Test]
        public void ShouldProcessFirmAddressCount()
        {
            var context = new Mock<IFactsContext>();
            context.SetupGet(x => x.Firms).Returns(Inquire(
                new Facts::Firm { Id = 1 },
                new Facts::Firm { Id = 2 }
                ));
            context.SetupGet(x => x.FirmAddresses).Returns(Inquire(
                new Facts::FirmAddress { Id = 1, FirmId = 2 }
                ));

            Transformation.Create(context.Object)
                .Transform(Operation.Create<CI::Firm>(1))
                .Transform(Operation.Create<CI::Firm>(2))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 1, AddressCount = 0 }, x => new { x.Id, x.AddressCount }))))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::Firm { Id = 2, AddressCount = 1 }, x => new { x.Id, x.AddressCount }))));
        }

        [Test]
        public void ShouldProcessFirmAccounts()
        {
            var facts = new Mock<ICustomerIntelligenceContext>();
            facts.SetupGet(x => x.Firms).Returns(Inquire(
                new CI::Firm { Id = 2 },
                new CI::Firm { Id = 3 }
                ));
            facts.SetupGet(x => x.FirmBalances).Returns(Inquire(
                new CI::FirmBalance { FirmId = 2, Balance = 2 },
                new CI::FirmBalance { FirmId = 2, Balance = 3 },
                new CI::FirmBalance { FirmId = 3, Balance = 1 }
                ));

            var ci = new Mock<ICustomerIntelligenceContext>();
            ci.SetupGet(x => x.Firms).Returns(Inquire(
                new CI::Firm { Id = 1 },
                new CI::Firm { Id = 2 }
                ));
            ci.SetupGet(x => x.FirmBalances).Returns(Inquire(
                new CI::FirmBalance { FirmId = 1, Balance = 1 },
                new CI::FirmBalance { FirmId = 2, Balance = 1 },
                new CI::FirmBalance { FirmId = 2, Balance = 2 }
                ));

            Transformation.Create(facts.Object, ci.Object)
                .Transform(Operation.Create<CI::Firm>(3))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 3, Balance = 1 }, x => new { x.FirmId, x.Balance }))), failMessage: "accounts should be added for an inserting firm");

            Transformation.Create(facts.Object, ci.Object)
                .Transform(Operation.Update<CI::Firm>(2))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 2, Balance = 1 }, x => new { x.FirmId, x.Balance }))), failMessage: "old accounts should be deleted for an updating firm")
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 2, Balance = 3 }, x => new { x.FirmId, x.Balance }))), failMessage: "new accounts should be added for an updating firm");

            Transformation.Create(facts.Object, ci.Object)
                .Transform(Operation.Delete<CI::Firm>(1))
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 1, Balance = 1 }, x => new { x.FirmId, x.Balance }))), failMessage: "accounts should be deleted for a deleting firm");
        }

        [Test]
        public void ShouldProcessFirmCategories()
        {
            var facts = new Mock<ICustomerIntelligenceContext>();
            facts.SetupGet(x => x.Firms).Returns(Inquire(
                new CI::Firm { Id = 2 },
                new CI::Firm { Id = 3 }
                ));
            facts.SetupGet(x => x.FirmCategories).Returns(Inquire(
                new CI::FirmCategory { FirmId = 2, CategoryId = 2 },
                new CI::FirmCategory { FirmId = 2, CategoryId = 3 },
                new CI::FirmCategory { FirmId = 3, CategoryId = 1 }
                ));

            var ci = new Mock<ICustomerIntelligenceContext>();
            ci.SetupGet(x => x.Firms).Returns(Inquire(
                new CI::Firm { Id = 1 },
                new CI::Firm { Id = 2 }
                ));
            ci.SetupGet(x => x.FirmCategories).Returns(Inquire(
                new CI::FirmCategory { FirmId = 1, CategoryId = 1 },
                new CI::FirmCategory { FirmId = 2, CategoryId = 1 },
                new CI::FirmCategory { FirmId = 2, CategoryId = 2 }
                ));

            Transformation.Create(facts.Object, ci.Object)
                .Transform(Operation.Create<CI::Firm>(3))
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 3, CategoryId = 1 }, x => new { x.FirmId, x.CategoryId }))), failMessage: "contacts should be added for an inserting firm");
                                                                                                                                                   
            Transformation.Create(facts.Object, ci.Object)                                                                                         
                .Transform(Operation.Update<CI::Firm>(2))                                                                                          
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 2, CategoryId = 1 }, x => new { x.FirmId, x.CategoryId }))), failMessage: "old contacts should be deleted for an updating firm")
                .Verify(m => m.Insert(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 2, CategoryId = 3 }, x => new { x.FirmId, x.CategoryId }))), failMessage: "new contacts should be added for an updating firm");
                                                                                                                                                   
            Transformation.Create(facts.Object, ci.Object)                                                                                         
                .Transform(Operation.Delete<CI::Firm>(1))                                                                                          
                .Verify(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory { FirmId = 1, CategoryId = 1 }, x => new { x.FirmId, x.CategoryId }))), failMessage: "contacts should be deleted for a deleting firm");
        }

        [TestCaseSource("Cases")]
        public void ShouldProcessChanges(Action test)
        {
            test();
        }

        private IEnumerable Cases
        {
            get
            {
                yield return Case(() => VerifyElementInsertion(new Facts::Firm { Id = 1 }, new CI::Firm { Id = 1 }, x => new { x.Id })).SetName("Should insert firm");
                yield return Case(() => VerifyElementInsertion(new Facts::Client { Id = 1 }, new CI::Client { Id = 1 }, x => new { x.Id })).SetName("Should insert client");
                yield return Case(() => VerifyElementInsertion(new Facts::Contact { Id = 1 }, new CI::Contact { Id = 1 }, x => new { x.Id })).SetName("Should insert contact");
                yield return Case(() => VerifyElementUpdate(new Facts::Firm { Id = 1 }, new CI::Firm { Id = 1 }, x => new { x.Id })).SetName("Should update firm");
                yield return Case(() => VerifyElementUpdate((new Facts::Client { Id = 1 }), new CI::Client { Id = 1 }, x => new { x.Id })).SetName("Should update client");
                yield return Case(() => VerifyElementUpdate((new Facts::Contact { Id = 1 }), new CI::Contact { Id = 1 }, x => new { x.Id })).SetName("Should update contact");
                yield return Case(() => VerifyElementDeletion(new Facts::Firm { Id = 1 }, new CI::Firm { Id = 1 }, x => new { x.Id })).SetName("Should delete firm");
                yield return Case(() => VerifyElementDeletion(new Facts::Client { Id = 1 }, new CI::Client { Id = 1 }, x => new { x.Id })).SetName("Should delete client");
                yield return Case(() => VerifyElementDeletion(new Facts::Contact { Id = 1 }, new CI::Contact { Id = 1 }, x => new { x.Id })).SetName("Should delete contact");
            }
        }

        private void VerifyElementInsertion<TSource, TTarget, TProjection>(TSource source, TTarget target, Func<TTarget, TProjection> projector)
            where TTarget : IIdentifiableObject
        {
            FactsConnection.Has(source);

            Transformation.Create(FactsConnection, CustomerIntelligenceConnection)
                .Transform(Operation.Create<TTarget>(target.Id))
                .Verify(m => m.Insert(It.Is(Predicate.Match(target, projector))), Times.Once, string.Format("The {0} element was not inserted.", typeof(TTarget).Name));
        }

        private void VerifyElementUpdate<TSource, TTarget, TProjection>(TSource source, TTarget target, Func<TTarget, TProjection> projector)
            where TTarget : IIdentifiableObject
        {
            FactsConnection.Has(source);
            CustomerIntelligenceConnection.Has(target);

            Transformation.Create(FactsConnection, CustomerIntelligenceConnection)
                .Transform(Operation.Update<TTarget>(target.Id))
                .Verify(m => m.Update(It.Is(Predicate.Match(target, projector))), Times.Once, string.Format("The {0} element was not updated.", typeof(TTarget).Name));
        }

        private void VerifyElementDeletion<TSource, TTarget, TProjection>(TSource source, TTarget target, Func<TTarget, TProjection> projector)
            where TTarget : IIdentifiableObject
        {
            CustomerIntelligenceConnection.Has(target);

            Transformation.Create(FactsConnection, CustomerIntelligenceConnection)
                .Transform(Operation.Delete<TTarget>(target.Id))
                .Verify(m => m.Delete(It.Is(Predicate.Match(target, projector))), Times.Once, string.Format("The {0} element was not deleted.", typeof(TTarget).Name));
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

            public Transformation Transform(params OperationInfo[] operations)
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