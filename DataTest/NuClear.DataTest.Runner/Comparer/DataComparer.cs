using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NuClear.DataTest.Metamodel;

namespace NuClear.DataTest.Runner.Comparer
{
    public sealed class DataComparer
    {
        private readonly SchemaMetadataElement _schemaMetadataElement;

        public DataComparer(SchemaMetadataElement schemaMetadataElement)
        {
            _schemaMetadataElement = schemaMetadataElement;
        }

        public CompareResult Compare(Type type, IReader actualReader, IReader expectedReader)
        {
            var helperType = typeof(ComparerHelper<>).MakeGenericType(type);
            var helperInstance = (IComparerHelper)Activator.CreateInstance(helperType, _schemaMetadataElement);
            return helperInstance.Compare(actualReader, expectedReader);
        }

        private interface IComparerHelper
        {
            CompareResult Compare(IReader actualReader, IReader expectedReader);
        }

        private class ComparerHelper<T> : IComparerHelper
            where T : class
        {
            private readonly EqualityComparer _identityComparer;
            private readonly EqualityComparer _completeComparer;

            public ComparerHelper(SchemaMetadataElement schemaMetadataElement)
            {
                _identityComparer = CreateIdentityComparer(schemaMetadataElement);
                _completeComparer = CreateCompleteComparer(schemaMetadataElement);
            }

            public CompareResult Compare(IReader actualReader, IReader expectedReader)
            {
                var actual = actualReader.Read<T>();
                var expected = expectedReader.Read<T>();

                var unexpected = actual.Except(expected, _identityComparer).ToArray();
                var missing = expected.Except(actual, _identityComparer).ToArray();
                var wrong = actual.Join(expected, x => x, x => x, Tuple.Create, _identityComparer)
                                  .Where(x => !_completeComparer.Equals(x.Item1, x.Item2))
                                  .Select(tuple => Tuple.Create<object, object>(tuple.Item1, tuple.Item2))
                                  .ToArray();

                return new CompareResult(unexpected, missing, wrong);
            }

            private static EqualityComparer CreateIdentityComparer(SchemaMetadataElement schemaMetadataElement)
            {
                var primaryKeys = schemaMetadataElement.Schema.GetEntityDescriptor(typeof(T)).Columns.Where(x => x.IsPrimaryKey).Select(x => x.MemberInfo);
                return new EqualityComparer(CreateIdentityComparisionExpression(primaryKeys).Compile());
            }

            private static EqualityComparer CreateCompleteComparer(SchemaMetadataElement schemaMetadataElement)
            {
                var primaryKeys = schemaMetadataElement.Schema.GetEntityDescriptor(typeof(T)).Columns.Select(x => x.MemberInfo);
                return new EqualityComparer(CreateIdentityComparisionExpression(primaryKeys).Compile());
            }

            private static Expression<Func<T, T, bool>> CreateIdentityComparisionExpression(IEnumerable<MemberInfo> primaryKeys)
            {
                // (T x, T y) => x.primaryKey1 == y.primaryKey1 && x.primaryKey2 == y.primaryKey2 && ...

                var parameterLeft = Expression.Parameter(typeof(T));
                var parameterRight = Expression.Parameter(typeof(T));

                var equalities = primaryKeys.Select(primaryKey =>
                {
                    var leftProperty = Expression.MakeMemberAccess(parameterLeft, primaryKey);
                    var rightProperty = Expression.MakeMemberAccess(parameterRight, primaryKey);
                    return Expression.Equal(leftProperty, rightProperty);
                });

                var keyComparision = equalities.Aggregate((BinaryExpression)null, (accumulator, expression) => accumulator != null ? Expression.And(accumulator, expression) : expression);


                return Expression.Lambda<Func<T, T, bool>>(keyComparision, parameterLeft, parameterRight);
            }

            private class EqualityComparer : IEqualityComparer<T>
            {
                private readonly Func<T, T, bool> _identityEquals;

                public EqualityComparer(Func<T, T, bool> identityEquals)
                {
                    _identityEquals = identityEquals;
                }

                public bool Equals(T x, T y)
                {
                    return _identityEquals.Invoke(x, y);
                }

                public int GetHashCode(T obj)
                {
                    // TODO {all, 03.11.2015}: Можно написать ещё один expression.
                    return 0;
                }
            }
        }
    }

    
}
