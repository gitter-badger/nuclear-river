using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Replication.API.Model;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    internal abstract class BaseFixture
    {
        protected static TestCaseData Case(Action action)
        {
            return new TestCaseData(action);
        }

        protected static class Predicate
        {
            public static Expression<Func<T, bool>> ById<T>(long id) where T : IIdentifiable
            {
                return item => item.Id == id;
            }

            public static Expression<Func<IEnumerable<T>, bool>> ByIds<T>(IEnumerable<long> ids) where T : IIdentifiable
            {
                return items => items.ToArray().Zip(ids, (item, id) => item.Id == id).All(x => x);
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
    }
}