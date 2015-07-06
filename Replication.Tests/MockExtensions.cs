using System;
using System.Linq;
using System.Linq.Expressions;

using Moq;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    static class MockExtensions
    {
        public static Mock<T> Setup<T, TProperty>(this Mock<T> mock,
                                                  Expression<Func<T, IQueryable<TProperty>>> prop, 
                                                  params TProperty[] values) where T : class
        {
            mock.Setup(prop).Returns(values.AsQueryable);
            return mock;
        }
    }
}