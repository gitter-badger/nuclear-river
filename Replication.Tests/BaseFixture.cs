using System;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Replication.Model;
using NuClear.Storage.Specifications;

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
            public static Expression<Func<T, bool>> ById<T>(long id) where T : IIdentifiable
            {
                return item => item.Id == id;
            }

            public static Expression<Func<T, bool>> Match<T>(T expected)
            {
                return Match(expected, x => x);
            }

            public static Expression<Func<T, bool>> Match<T, TProjection>(T expected, Func<T, TProjection> projector)
            {
                return item => new ProjectionEqualityComparer<T, TProjection>(projector).Equals(item, expected);
            }
        }

        #endregion

        public static class TestSpecs
        {
            public static class Find
            {
                public static FindSpecification<T> ById<T>(long id) where T : IIdentifiable
                {
                    return new FindSpecification<T>(x => x.Id == id);
                }
            }
        }
    }
}