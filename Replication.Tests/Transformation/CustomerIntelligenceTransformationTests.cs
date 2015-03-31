using System;
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
            context.SetupGet(x => x.Firms).Returns(Enumerate(
                new Facts::Firm { Id = 1, LastDisqualifiedOn = firmDate },
                new Facts::Firm { Id = 2, LastDisqualifiedOn = firmDate , ClientId = 1}
                ));
            context.SetupGet(x => x.Clients).Returns(Enumerate(
                new Facts::Client { Id = 1, LastDisqualifiedOn = clientDate }
                ));

            Transformation.Create(context.Object)
                .Transform(Operation.Create<CI::Firm>(1))
                .Transform(Operation.Create<CI::Firm>(2))
                .Verify(m => m.Insert(It.Is(Arg.Match(new CI::Firm { Id = 1, LastDisqualifiedOn = firmDate }, x => new { x.Id, x.LastDisqualifiedOn }))))
                .Verify(m => m.Insert(It.Is(Arg.Match(new CI::Firm { Id = 2, LastDisqualifiedOn = clientDate }, x => new { x.Id, x.LastDisqualifiedOn }))));
        }

        [Test]
        public void ShouldProcessFirmDistributionDate()
        {
            var date = new DateTimeOffset(2015, 2, 1, 12, 30, 0, new TimeSpan());

            var context = new Mock<IFactsContext>();
            context.SetupGet(x => x.Firms).Returns(Enumerate(
                new Facts::Firm { Id = 1 },
                new Facts::Firm { Id = 2 }
                ));
            context.SetupGet(x => x.Orders).Returns(Enumerate(
                new Facts::Order { Id = 1, EndDistributionDateFact = date, WorkflowStepId = 4, FirmId = 2 }
                ));

            Transformation.Create(context.Object)
                .Transform(Operation.Create<CI::Firm>(1))
                .Transform(Operation.Create<CI::Firm>(2))
                .Verify(m => m.Insert(It.Is(Arg.Match(new CI::Firm { Id = 1, LastDistributedOn = null }, x => new { x.Id, x.LastDistributedOn }))))
                .Verify(m => m.Insert(It.Is(Arg.Match(new CI::Firm { Id = 2, LastDistributedOn = date }, x => new { x.Id, x.LastDistributedOn }))));
        }

        [Test]
        public void ShouldProcessFirmAddressCount()
        {
            var context = new Mock<IFactsContext>();
            context.SetupGet(x => x.Firms).Returns(Enumerate(
                new Facts::Firm { Id = 1 },
                new Facts::Firm { Id = 2 }
                ));
            context.SetupGet(x => x.FirmAddresses).Returns(Enumerate(
                new Facts::FirmAddress { Id = 1, FirmId = 2 }
                ));

            Transformation.Create(context.Object)
                .Transform(Operation.Create<CI::Firm>(1))
                .Transform(Operation.Create<CI::Firm>(2))
                .Verify(m => m.Insert(It.Is(Arg.Match(new CI::Firm { Id = 1, AddressCount = 0 }, x => new { x.Id, x.AddressCount }))))
                .Verify(m => m.Insert(It.Is(Arg.Match(new CI::Firm { Id = 2, AddressCount = 1 }, x => new { x.Id, x.AddressCount }))));
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

                var date = new DateTimeOffset(2015, 1, 1, 12, 30, 0, new TimeSpan());

                yield return Case(() => VerifyElementInsertion(new Facts::Firm { Id = 1, Name = "firm" }, new CI::Firm { Id = 1, Name = "firm" }, x => new { x.Id, x.Name })).SetName("Should process name");
                yield return Case(() => VerifyElementInsertion(new Facts::Firm { Id = 1, CreatedOn = date }, new CI::Firm { Id = 1, CreatedOn = date }, x => new { x.Id, x.CreatedOn })).SetName("Should process createdOn");
                yield return Case(() => VerifyElementInsertion(new Facts::Firm { Id = 1, ClientId = 123 }, new CI::Firm { Id = 1, ClientId = 123 }, x => new { x.Id, x.ClientId })).SetName("Should process clientId");
                yield return Case(() => VerifyElementInsertion(new Facts::Firm { Id = 1, OrganizationUnitId = 123 }, new CI::Firm { Id = 1, OrganizationUnitId = 123 }, x => new { x.Id, x.OrganizationUnitId })).SetName("Should process organizationUnitId");
                yield return Case(() => VerifyElementInsertion(new Facts::Firm { Id = 1, TerritoryId = 123 }, new CI::Firm { Id = 1, TerritoryId = 123 }, x => new { x.Id, x.TerritoryId })).SetName("Should process territoryId");
            }
        }

        private void VerifyElementInsertion<TSource, TTarget, TProjection>(TSource source, TTarget target, Func<TTarget, TProjection> projector)
            where TTarget : IIdentifiable
        {
            FactsConnection.Has(source);

            Transformation.Create(FactsConnection)
                .Transform(Operation.Create<TTarget>(target.Id))
                .Verify(m => m.Insert(It.Is(Arg.Match(target, projector))), Times.Once, string.Format("The {0} element was not inserted.", typeof(TTarget).Name));
        }

        private void VerifyElementUpdate<TSource, TTarget, TProjection>(TSource source, TTarget target, Func<TTarget, TProjection> projector)
            where TTarget : IIdentifiable
        {
            FactsConnection.Has(source);
            CustomerIntelligenceConnection.Has(target);

            Transformation.Create(FactsConnection)
                .Transform(Operation.Update<TTarget>(target.Id))
                .Verify(m => m.Update(It.Is(Arg.Match(target, projector))), Times.Once, string.Format("The {0} element was not updated.", typeof(TTarget).Name));
        }

        private void VerifyElementDeletion<TSource, TTarget, TProjection>(TSource source, TTarget target, Func<TTarget, TProjection> projector)
            where TTarget : IIdentifiable
        {
            CustomerIntelligenceConnection.Has(target);

            Transformation.Create(FactsConnection, CustomerIntelligenceConnection)
                .Transform(Operation.Delete<TTarget>(target.Id))
                .Verify(m => m.Delete(It.Is(Arg.Match(target, projector))), Times.Once, string.Format("The {0} element was not deleted.", typeof(TTarget).Name));
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