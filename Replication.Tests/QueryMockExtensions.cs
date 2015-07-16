using System;
using System.Linq;
using System.Linq.Expressions;

using Moq;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    internal static class QueryMockExtensions
    {
        public static Mock<IQuery> Setup<TResult>(this Mock<IQuery> mock,
                                                  Expression<Func<IQuery, IQueryable<TResult>>> expression,
                                                  params TResult[] results)
        {
            var visitor = new ForMethodCallVisitor();

            expression = (Expression<Func<IQuery, IQueryable<TResult>>>)visitor.Visit(expression);
            if (visitor.HasParams)
            {
                mock.Setup(expression).Returns<FindSpecification<TResult>>(spec => results.AsQueryable().Where(spec));
            }
            else
            {
                mock.Setup(expression).Returns(results.AsQueryable);
            }

            return mock;
        }

        public static Mock<IQuery> Setup<TResult>(this Mock<IQuery> mock,
                                                  Expression<Func<IQuery, IQueryable<TResult>>> expression1,
                                                  Expression<Func<IQuery, IQueryable<TResult>>> expression2,
                                                  params TResult[] results)
        {
            var visitor = new ForMethodCallVisitor();
            
            expression1 = (Expression<Func<IQuery, IQueryable<TResult>>>)visitor.Visit(expression1);
            if (visitor.HasParams)
            {
                mock.Setup(expression1).Returns<FindSpecification<TResult>>(spec => results.AsQueryable().Where(spec));
            }
            else
            {
                mock.Setup(expression1).Returns(results.AsQueryable);
            }

            expression2 = (Expression<Func<IQuery, IQueryable<TResult>>>)visitor.Visit(expression2);
            if (visitor.HasParams)
            {
                mock.Setup(expression2).Returns<FindSpecification<TResult>>(spec => results.AsQueryable().Where(spec));
            }
            else
            {
                mock.Setup(expression2).Returns(results.AsQueryable);
            }
            
            return mock;
        }

        private class ForMethodCallVisitor : ExpressionVisitor
        {
            public bool HasParams { get; private set; }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.Name == "For")
                {
                    HasParams = node.Arguments.Any();
                }

                return base.VisitMethodCall(node);
            }
        }
    }
}