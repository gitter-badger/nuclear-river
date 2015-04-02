using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    internal static class IdentifiableObjectExtensions
    {
        public static IEnumerable<T> ById<T>(this IEnumerable<T> objects, params long[] ids) where T : IIdentifiableObject
        {
            return objects.Where(x => ids.Contains(x.Id));
        }
    }
}