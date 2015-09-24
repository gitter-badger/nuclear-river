using System;
using System.Collections.Generic;

namespace NuClear.Replication.Metadata.Model
{
    public sealed class IdentifiableObjectEqualityComparer<T> : IEqualityComparer<T>
        where T : IIdentifiable
    {
        private static volatile IdentifiableObjectEqualityComparer<T> _defaultComparer;

        public static IdentifiableObjectEqualityComparer<T> Default
        {
            get
            {
                var equalityComparer = _defaultComparer;
                if (equalityComparer == null)
                {
                    _defaultComparer = equalityComparer = new IdentifiableObjectEqualityComparer<T>();
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
            if (obj.Equals(default(T)))
            {
                throw new ArgumentNullException("obj");
            }

            return obj.Id.GetHashCode();
        }
    }
}