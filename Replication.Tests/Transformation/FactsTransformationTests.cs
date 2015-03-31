using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB;

using Moq;

using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Data.Context;
using NuClear.AdvancedSearch.Replication.Model;
using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;
using NuClear.AdvancedSearch.Replication.Tests.Data;
using NuClear.AdvancedSearch.Replication.Transforming;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
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
                yield return CaseToVerifyElementInsertion<Erm.Account, Fact.Account>();
                yield return CaseToVerifyElementInsertion<Erm.CategoryFirmAddress, Fact.CategoryFirmAddress>();
                yield return CaseToVerifyElementInsertion<Erm.CategoryOrganizationUnit, Fact.CategoryOrganizationUnit>();
                yield return CaseToVerifyElementInsertion<Erm.Client, Fact.Client>();
                yield return CaseToVerifyElementInsertion<Erm.Contact, Fact.Contact>();
                yield return CaseToVerifyElementInsertion<Erm.Firm, Fact.Firm>();
                yield return CaseToVerifyElementInsertion<Erm.FirmAddress, Fact.FirmAddress>();
                yield return CaseToVerifyElementInsertion<Erm.FirmContact, Fact.FirmContact>();
                yield return CaseToVerifyElementInsertion<Erm.LegalPerson, Fact.LegalPerson>();
                yield return CaseToVerifyElementInsertion<Erm.Order, Fact.Order>();
                // update
                yield return CaseToVerifyElementUpdate<Erm.Account, Fact.Account>();
                yield return CaseToVerifyElementUpdate<Erm.CategoryFirmAddress, Fact.CategoryFirmAddress>();
                yield return CaseToVerifyElementUpdate<Erm.CategoryOrganizationUnit, Fact.CategoryOrganizationUnit>();
                yield return CaseToVerifyElementUpdate<Erm.Client, Fact.Client>();
                yield return CaseToVerifyElementUpdate<Erm.Contact, Fact.Contact>();
                yield return CaseToVerifyElementUpdate<Erm.Firm, Fact.Firm>();
                yield return CaseToVerifyElementUpdate<Erm.FirmAddress, Fact.FirmAddress>();
                yield return CaseToVerifyElementUpdate<Erm.FirmContact, Fact.FirmContact>();
                yield return CaseToVerifyElementUpdate<Erm.LegalPerson, Fact.LegalPerson>();
                yield return CaseToVerifyElementUpdate<Erm.Order, Fact.Order>();
                // delete
                yield return CaseToVerifyElementDeletion<Erm.Account, Fact.Account>();
                yield return CaseToVerifyElementDeletion<Erm.CategoryFirmAddress, Fact.CategoryFirmAddress>();
                yield return CaseToVerifyElementDeletion<Erm.CategoryOrganizationUnit, Fact.CategoryOrganizationUnit>();
                yield return CaseToVerifyElementDeletion<Erm.Client, Fact.Client>();
                yield return CaseToVerifyElementDeletion<Erm.Contact, Fact.Contact>();
                yield return CaseToVerifyElementDeletion<Erm.Firm, Fact.Firm>();
                yield return CaseToVerifyElementDeletion<Erm.FirmAddress, Fact.FirmAddress>();
                yield return CaseToVerifyElementDeletion<Erm.FirmContact, Fact.FirmContact>();
                yield return CaseToVerifyElementDeletion<Erm.LegalPerson, Fact.LegalPerson>();
                yield return CaseToVerifyElementDeletion<Erm.Order, Fact.Order>();
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
                var transformation = new Transformation(source, target);


                return transformation;
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

    internal static class DataContextExtensions
    {
        public static IDataContext Has<T>(this IDataContext context, long id) where T : IIdentifiable, new()
        {
            return context.Has(Create<T>(id));
        }

        public static IDataContext Has<T>(this IDataContext context, T obj)
        {
            using(new NoSqlTrace())
            {
                context.Insert(obj);
                return context;
            }
        }

        public static T Read<T>(this IDataContext context, long id) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var predicate = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Property(parameter, "Id"), Expression.Constant(id)), parameter);

            return context.GetTable<T>().FirstOrDefault(predicate);
        }

        private static T Create<T>(long id) where T : IIdentifiable, new()
        {
            var obj = new T();
            obj.GetType().GetProperty("Id").SetValue(obj, id);
            return obj;
        }

        private static Object Create(Type type, long id)
        {
            var obj = Activator.CreateInstance(type);
            obj.GetType().GetProperty("Id").SetValue(obj, id);
            return obj;
        }
    }
}