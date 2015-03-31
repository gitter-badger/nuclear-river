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

            var ermContext = new Mock<IErmContext>();
            ermContext.SetupGet(x => x.Contacts).Returns(Enumerate(new Erm::Contact { Id = entityId, Role = ermRole }));

            Transformation.Create(ermContext.Object)
                .Transform(Operation.Create<Facts::Contact>(entityId))
                .Verify(m => m.Insert(It.Is<Facts::Contact>(contact => contact.Id == entityId && contact.Role == expectedRole)));
        }

        [Test]
        public void ShouldProcessContactPhones()
        {
            var ermContext = new Mock<IErmContext>();
            ermContext.SetupGet(x => x.Contacts).Returns(Enumerate(
                new Erm::Contact { Id = 1 },
                new Erm::Contact { Id = 2, MainPhoneNumber = "<phone>" },
                new Erm::Contact { Id = 3, AdditionalPhoneNumber = "<phone>" },
                new Erm::Contact { Id = 4, MobilePhoneNumber = "<phone>" },
                new Erm::Contact { Id = 5, HomePhoneNumber = "<phone>" }
                ));

            Transformation.Create(ermContext.Object)
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

            ermContext.VerifyGet(x => x.Contacts, Times.Exactly(5), "The transformation was applied sequentially (not in bulk).");
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
                // insert
                yield return CaseToVerifyElementInsertion<Erm::Account, Facts::Account>();
                yield return CaseToVerifyElementInsertion<Erm::CategoryFirmAddress, Facts::CategoryFirmAddress>();
                yield return CaseToVerifyElementInsertion<Erm::CategoryOrganizationUnit, Facts::CategoryOrganizationUnit>();
                yield return CaseToVerifyElementInsertion<Erm::Client, Facts::Client>();
                yield return CaseToVerifyElementInsertion<Erm::Contact, Facts::Contact>();
                yield return CaseToVerifyElementInsertion<Erm::Firm, Facts::Firm>();
                yield return CaseToVerifyElementInsertion<Erm::FirmAddress, Facts::FirmAddress>();
                yield return CaseToVerifyElementInsertion<Erm::FirmContact, Facts::FirmContact>();
                yield return CaseToVerifyElementInsertion<Erm::LegalPerson, Facts::LegalPerson>();
                yield return CaseToVerifyElementInsertion<Erm::Order, Facts::Order>();
                // update
                yield return CaseToVerifyElementUpdate<Erm::Account, Facts::Account>();
                yield return CaseToVerifyElementUpdate<Erm::CategoryFirmAddress, Facts::CategoryFirmAddress>();
                yield return CaseToVerifyElementUpdate<Erm::CategoryOrganizationUnit, Facts::CategoryOrganizationUnit>();
                yield return CaseToVerifyElementUpdate<Erm::Client, Facts::Client>();
                yield return CaseToVerifyElementUpdate<Erm::Contact, Facts::Contact>();
                yield return CaseToVerifyElementUpdate<Erm::Firm, Facts::Firm>();
                yield return CaseToVerifyElementUpdate<Erm::FirmAddress, Facts::FirmAddress>();
                yield return CaseToVerifyElementUpdate<Erm::FirmContact, Facts::FirmContact>();
                yield return CaseToVerifyElementUpdate<Erm::LegalPerson, Facts::LegalPerson>();
                yield return CaseToVerifyElementUpdate<Erm::Order, Facts::Order>();
                // delete
                yield return CaseToVerifyElementDeletion<Erm::Account, Facts::Account>();
                yield return CaseToVerifyElementDeletion<Erm::CategoryFirmAddress, Facts::CategoryFirmAddress>();
                yield return CaseToVerifyElementDeletion<Erm::CategoryOrganizationUnit, Facts::CategoryOrganizationUnit>();
                yield return CaseToVerifyElementDeletion<Erm::Client, Facts::Client>();
                yield return CaseToVerifyElementDeletion<Erm::Contact, Facts::Contact>();
                yield return CaseToVerifyElementDeletion<Erm::Firm, Facts::Firm>();
                yield return CaseToVerifyElementDeletion<Erm::FirmAddress, Facts::FirmAddress>();
                yield return CaseToVerifyElementDeletion<Erm::FirmContact, Facts::FirmContact>();
                yield return CaseToVerifyElementDeletion<Erm::LegalPerson, Facts::LegalPerson>();
                yield return CaseToVerifyElementDeletion<Erm::Order, Facts::Order>();
            }
        }

        private TestCaseData CaseToVerifyElementInsertion<TErmElement, TFactElement>()
            where TErmElement : IIdentifiable, new()
            where TFactElement : IIdentifiable, new()
        {
            return Case(VerifyElementInsertion<TErmElement, TFactElement>).SetName(string.Format("Should process and insert {0} element.", typeof(TFactElement).Name));
        }

        private TestCaseData CaseToVerifyElementUpdate<TErmElement, TFactElement>()
            where TErmElement : IIdentifiable, new()
            where TFactElement : IIdentifiable, new()
        {
            return Case(VerifyElementUpdate<TErmElement, TFactElement>).SetName(string.Format("Should process and update {0} element.", typeof(TFactElement).Name));
        }

        private TestCaseData CaseToVerifyElementDeletion<TErmElement, TFactElement>()
            where TErmElement : IIdentifiable, new()
            where TFactElement : IIdentifiable, new()
        {
            return Case(VerifyElementDeletion<TErmElement, TFactElement>).SetName(string.Format("Should process and delete {0} element.", typeof(TFactElement).Name));
        }

        private void VerifyElementInsertion<TErmElement, TFactElement>()
            where TErmElement : IIdentifiable, new()
            where TFactElement : IIdentifiable, new()
        {
            const int entityId = 1;
            ErmConnection.Has<TErmElement>(entityId);

            Transformation.Create(ErmConnection, FactsConnection)
                          .Transform(Operation.Create<TFactElement>(entityId))
                          .Verify(x => x.Insert(HasId<TFactElement>(entityId)), Times.Once, string.Format("The {0} element was not inserted.", typeof(TFactElement).Name));
        }

        private void VerifyElementUpdate<TErmElement, TFactElement>()
            where TErmElement : IIdentifiable, new()
            where TFactElement : IIdentifiable, new()
        {
            const long entityId = 1;
            ErmConnection.Has<TErmElement>(entityId);
            FactsConnection.Has<TFactElement>(entityId);

            Transformation.Create(ErmConnection, FactsConnection)
                          .Transform(Operation.Update<TFactElement>(entityId))
                          .Verify(x => x.Update(HasId<TFactElement>(entityId)), Times.Once, string.Format("The {0} element was not updated.", typeof(TFactElement).Name));
        }

        private void VerifyElementDeletion<TErmElement, TFactElement>()
            where TErmElement : IIdentifiable, new()
            where TFactElement : IIdentifiable, new()
        {
            const long entityId = 1;
            FactsConnection.Has<TFactElement>(entityId);

            Transformation.Create(ErmConnection, FactsConnection)
                          .Transform(Operation.Delete<TFactElement>(entityId))
                          .Verify(x => x.Delete(HasId<TFactElement>(entityId)), Times.Once, string.Format("The {0} element was not deleted.", typeof(TFactElement).Name));
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