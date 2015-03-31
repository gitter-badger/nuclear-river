using System;
using System.Linq;
using System.Linq.Expressions;

using Moq;

using Newtonsoft.Json;

using NuClear.AdvancedSearch.Replication.Model;
using NuClear.AdvancedSearch.Replication.Transforming;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    internal abstract class BaseTransformationFixture : BaseFixture
    {
        protected static TestCaseData Case(Action action)
        {
            return new TestCaseData(action);
        }

        protected static IQueryable<T> Enumerate<T>(params T[] elements)
        {
            return elements.AsQueryable();
        }

        protected static T HasId<T>(long id) where T : IIdentifiable
        {
            return It.Is(Arg.ById<T>(id));
        }

        #region Arg

        protected static class Arg
        {
            public static Expression<Func<T, bool>> ById<T>(long id) where T : IIdentifiable
            {
                return item => item.Id == id;
            }

            public static Expression<Func<T, bool>> Match<T, TProjection>(T expected, Func<T, TProjection> projector)
            {
                Func<T, string> converter = x => Serialize(projector(x));

                var expectedAsString = converter(expected);

                return item => string.CompareOrdinal(converter(item), expectedAsString) == 0;
            }

            private static string Serialize<T>(T obj)
            {
                return JsonConvert.SerializeObject(obj);
            }
        }

        #endregion

        #region Operation

        protected static class Operation
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

        #endregion
    }
}