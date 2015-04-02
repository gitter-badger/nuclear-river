using System;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Replication.Model;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    internal abstract class BaseFixture
    {
        protected static TestCaseData Case(Action action)
        {
            return new TestCaseData(action);
        }

        #region Predicate

        protected static class Predicate
        {
            public static Expression<Func<T, bool>> ById<T>(long id) where T : IIdentifiableObject
            {
                return item => item.Id == id;
            }

            public static Expression<Func<T, bool>> Match<T, TProjection>(T expected, Func<T, TProjection> projector)
            {
                return item => new ProjectionEqualityComparer<T, TProjection>(projector).Equals(item, expected);
            }
        }

        #endregion
    }
}