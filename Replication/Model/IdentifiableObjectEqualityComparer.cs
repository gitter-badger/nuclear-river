using System;
using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Replication.Model
{
    internal sealed class IdentifiableObjectEqualityComparer<T> : IEqualityComparer<T>
        where T : IIdentifiableObject
    {
        private static volatile IdentifiableObjectEqualityComparer<T> defaultComparer;

        public static IdentifiableObjectEqualityComparer<T> Default
        {
            get
            {
                var equalityComparer = defaultComparer;
                if (equalityComparer == null)
                {
                    defaultComparer = equalityComparer = new IdentifiableObjectEqualityComparer<T>();
                }
                return equalityComparer;
            }
        }

        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (ReferenceEquals(x, null))
            {
                return false;
            }
            if (ReferenceEquals(y, null))
            {
                return false;
            }
            if (x.GetType() != y.GetType())
            {
                return false;
            }
            return x.Id == y.Id;
        }

        public int GetHashCode(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            return obj.Id.GetHashCode();
        }
    }
}