using System;
using System.Collections;
using System.Linq.Expressions;

using LinqToDB;

using Moq;

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
    internal class FactsTransformationTests : BaseFixture
    {
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

        private static TestCaseData Case(Action action)
        {
            return new TestCaseData(action);
        }

        private void VerifyElementInsertion<TErmElement, TFactElement>()
            where TErmElement : IIdentifiable, new()
            where TFactElement : IIdentifiable, new()
        {
            const int entityId = 1;
            ErmConnection.Has<TErmElement>(entityId);

            Transformation.Create(ErmConnection, FactsConnection)
                          .Transform(Operation.Create<TFactElement>(entityId))
                          .VerifyInsertion<TFactElement>(entityId, Times.Once, string.Format("The {0} element was not inserted.", typeof(TFactElement).Name));
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
                          .VerifyUpdate<TFactElement>(entityId, Times.Once, string.Format("The {0} element was not updated.", typeof(TFactElement).Name));
        }

        private void VerifyElementDeletion<TErmElement, TFactElement>()
            where TErmElement : IIdentifiable, new()
            where TFactElement : IIdentifiable, new()
        {
            const long entityId = 1;
            FactsConnection.Has<TFactElement>(entityId);

            Transformation.Create(ErmConnection, FactsConnection)
                          .Transform(Operation.Delete<TFactElement>(entityId))
                          .VerifyDelete<TFactElement>(entityId, Times.Once, string.Format("The {0} element was not deleted.", typeof(TFactElement).Name));
        }

        private class Transformation
        {
            private readonly FactsTransformation _transformation;
            private readonly Mock<IDataMapper> _mapper;

            private Transformation(IDataContext source, IDataContext target)
            {
                var erm = new ErmContext(source);
                var facts = new FactsContext(target);

                _mapper = new Mock<IDataMapper>();
                _transformation = new FactsTransformation(new FactsTransformationContext(erm), facts, _mapper.Object);
            }

            public static Transformation Create(IDataContext source, IDataContext target)
            {
                return new Transformation(source, target);
            }

            public Transformation Transform(params OperationInfo[] operations)
            {
                _transformation.Transform(operations);
                return this;
            }

            public Transformation VerifyInsertion<T>(long id, Func<Times> times, string failMessage = null) where T : IIdentifiable
            {
                return Verify(x => x.Insert(HasId<T>(id)), times, failMessage);
            }

            public Transformation VerifyUpdate<T>(long id, Func<Times> times, string failMessage = null) where T : IIdentifiable
            {
                return Verify(x => x.Update(HasId<T>(id)), times, failMessage);
            }

            public Transformation VerifyDelete<T>(long id, Func<Times> times, string failMessage = null) where T : IIdentifiable
            {
                return Verify(x => x.Delete(HasId<T>(id)), times, failMessage);
            }

            public Transformation Verify(Expression<Action<IDataMapper>> action, Func<Times> times, string failMessage = null)
            {
                _mapper.Verify(action, times, failMessage);
                return this;
            }
        }

        private static class Operation
        {
            public static OperationInfo Create<T>(long entityId)
            {
                return Build<T>(Transforming.Operation.Created, entityId);
            }

            public static OperationInfo Update<T>(long entityId)
            {
                return Build<T>(Transforming.Operation.Updated, entityId);
            }

            public static OperationInfo Delete<T>(long entityId)
            {
                return Build<T>(Transforming.Operation.Deleted, entityId);
            }

            private static OperationInfo Build<T>(Transforming.Operation operation, long entityId)
            {
                return new OperationInfo(operation, typeof(T), entityId);
            }
        }

        private static T HasId<T>(long id) where T : IIdentifiable
        {
            return It.Is<T>(item => item.Id == id);
        }
    }
}