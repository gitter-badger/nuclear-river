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
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal class FactsTransformationTests : BaseTransformationFixture
    {
        [TestCase(200000, 1, TestName = "Should convert 'Employee' value.")]
        [TestCase(200001, 2, TestName = "Should convert 'InfluenceDecisions' value.")]
        [TestCase(200002, 3, TestName = "Should convert 'MakingDecisions' value.")]
        [TestCase(Int32.MinValue, 0, TestName = "Should not fail for an unknown value.")]
        public void ShouldConvertContactRole(int ermRole, int expectedRole)
        {
            const int entityId = 1;

            var ermContext = Mock.Of<IErmContext>(ctx => ctx.Contacts == Inquire(new Erm::Contact { Id = entityId, Role = ermRole }));

            Transformation.Create(ermContext)
                .Transform(Operation.Create<Facts::Contact>(entityId))
                .Verify(m => m.Insert(It.Is<Facts::Contact>(contact => contact.Id == entityId && contact.Role == expectedRole)));
        }

        [Test]
        public void ShouldProcessContactPhones()
        {
            var ermContext = Mock.Of<IErmContext>(
                ctx => ctx.Contacts == Inquire(
                    new Erm::Contact { Id = 1 },
                    new Erm::Contact { Id = 2, MainPhoneNumber = "<phone>" },
                    new Erm::Contact { Id = 3, AdditionalPhoneNumber = "<phone>" },
                    new Erm::Contact { Id = 4, MobilePhoneNumber = "<phone>" },
                    new Erm::Contact { Id = 5, HomePhoneNumber = "<phone>" }));

            Transformation.Create(ermContext)
                .Transform(Operation.Create<Facts::Contact>(1))
                .Transform(Operation.Create<Facts::Contact>(2))
                .Transform(Operation.Create<Facts::Contact>(3))
                .Transform(Operation.Create<Facts::Contact>(4))
                .Transform(Operation.Create<Facts::Contact>(5))
                .Verify(m => m.Insert(It.Is<Facts::Contact>(contact => contact.Id == 1 && !contact.HasPhone)))
                .Verify(m => m.Insert(It.Is<Facts::Contact>(contact => contact.Id == 2 && contact.HasPhone)))
                .Verify(m => m.Insert(It.Is<Facts::Contact>(contact => contact.Id == 3 && contact.HasPhone)))
                .Verify(m => m.Insert(It.Is<Facts::Contact>(contact => contact.Id == 4 && contact.HasPhone)))
                .Verify(m => m.Insert(It.Is<Facts::Contact>(contact => contact.Id == 5 && contact.HasPhone)));

            Mock.Get(ermContext).VerifyGet(x => x.Contacts, Times.Exactly(5), "The transformation was applied sequentially (not in bulk).");
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
                const int notnull = 1;

                // insert
                yield return CaseToVerifyElementInsertion<Erm.Account, Facts.Account>(new Erm::Account { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm.CategoryFirmAddress, Facts.CategoryFirmAddress>(new Erm::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm.CategoryOrganizationUnit, Facts.CategoryOrganizationUnit>(new Erm::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm.Client, Facts.Client>(new Erm::Client { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm.Contact, Facts.Contact>(new Erm::Contact { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm.Firm, Facts.Firm>(new Erm::Firm { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm.FirmAddress, Facts.FirmAddress>(new Erm::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm.FirmContact, Facts.FirmContact>(new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = notnull });
                yield return CaseToVerifyElementInsertion<Erm.LegalPerson, Facts.LegalPerson>(new Erm::LegalPerson { Id = 1, ClientId = notnull });
                yield return CaseToVerifyElementInsertion<Erm.Order, Facts.Order>(new Erm::Order { Id = 1, WorkflowStepId = 4 });
                // update
                yield return CaseToVerifyElementUpdate(new Erm::Account { Id = 1 }, new Facts::Account { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::CategoryFirmAddress { Id = 1 }, new Facts::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::CategoryOrganizationUnit { Id = 1 }, new Facts::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Client { Id = 1 }, new Facts::Client { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Contact { Id = 1 }, new Facts::Contact { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Firm { Id = 1 }, new Facts::Firm { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::FirmAddress { Id = 1 }, new Facts::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = notnull }, new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = notnull });
                yield return CaseToVerifyElementUpdate(new Erm::LegalPerson { Id = 1, ClientId = notnull }, new Facts::LegalPerson { Id = 1, ClientId = notnull });
                yield return CaseToVerifyElementUpdate(new Erm::Order { Id = 1, WorkflowStepId = 4 }, new Facts::Order { Id = 1 });
                // delete
                yield return CaseToVerifyElementDeletion(new Facts::Account { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Client { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Contact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Firm { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::FirmContact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::LegalPerson { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Order { Id = 1 });
            }
        }

        private TestCaseData CaseToVerifyElementInsertion<TErmElement, TFactElement>(TErmElement source)
            where TErmElement : IIdentifiableObject, new()
            where TFactElement : IIdentifiableObject, new()
        {
            return Case(() => VerifyElementInsertion<TErmElement, TFactElement>(source)).SetName(string.Format("Should process and insert {0} element.", typeof(TFactElement).Name));
        }

        private TestCaseData CaseToVerifyElementUpdate<TErmElement, TFactElement>(TErmElement source, TFactElement target)
            where TErmElement : IIdentifiableObject, new()
            where TFactElement : IIdentifiableObject, new()
        {
            return Case(() => VerifyElementUpdate(source, target)).SetName(string.Format("Should process and update {0} element.", typeof(TFactElement).Name));
        }

        private TestCaseData CaseToVerifyElementDeletion<TFactElement>(TFactElement target) where TFactElement : IIdentifiableObject, new()
        {
            return Case(() => VerifyElementDeletion(target)).SetName(string.Format("Should process and delete {0} element.", typeof(TFactElement).Name));
        }

        private void VerifyElementInsertion<TErmElement, TFactElement>(TErmElement source)
            where TErmElement : IIdentifiableObject, new()
            where TFactElement : IIdentifiableObject, new()
        {
            var entityId = source.Id;
            ErmConnection.Has(source);

            Transformation.Create(ErmConnection, FactsConnection)
                          .Transform(Operation.Create<TFactElement>(entityId))
                          .Verify(x => x.Insert(It.Is(Predicate.ById<TFactElement>(entityId))), Times.Once, string.Format("The {0} element was not inserted.", typeof(TFactElement).Name));
        }

        private void VerifyElementUpdate<TErmElement, TFactElement>(TErmElement source, TFactElement target)
            where TErmElement : IIdentifiableObject, new()
            where TFactElement : IIdentifiableObject, new()
        {
            ErmConnection.Has(source);
            FactsConnection.Has(target);

            Transformation.Create(ErmConnection, FactsConnection)
                          .Transform(Operation.Update<TFactElement>(target.Id))
                          .Verify(x => x.Update(It.Is(Predicate.ById<TFactElement>(target.Id))), Times.Once, string.Format("The {0} element was not updated.", typeof(TFactElement).Name));
        }

        private void VerifyElementDeletion<TFactElement>(TFactElement target) where TFactElement : IIdentifiableObject, new()
        {
            FactsConnection.Has(target);

            Transformation.Create(ErmConnection, FactsConnection)
                          .Transform(Operation.Delete<TFactElement>(target.Id))
                          .Verify(x => x.Delete(It.Is(Predicate.ById<TFactElement>(target.Id))), Times.Once, string.Format("The {0} element was not deleted.", typeof(TFactElement).Name));
        }

        #region Transformation

        private class Transformation
        {
            private readonly FactsTransformation _transformation;
            private readonly Mock<IDataMapper> _mapper;

            private Transformation(IFactsContext source, IFactsContext target)
            {
                _mapper = new Mock<IDataMapper>();
                _transformation = new FactsTransformation(source, target, _mapper.Object);
            }

            public static Transformation Create(IDataContext source = null, IDataContext target = null)
            {
                return Create(
                    new ErmContext(source ?? new Mock<IDataContext>().Object),
                    new FactsContext(target ?? new Mock<IDataContext>().Object));
            }

            public static Transformation Create(IErmContext source = null, IFactsContext target = null)
            {
                return Create(new FactsTransformationContext(source ?? new Mock<IErmContext>().Object), target);
            }

            public static Transformation Create(IFactsContext source = null, IFactsContext target = null)
            {
                return new Transformation(source ?? new Mock<IFactsContext>().Object, target ?? new Mock<IFactsContext>().Object);
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