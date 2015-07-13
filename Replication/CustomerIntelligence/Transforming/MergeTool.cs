using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using LinqToDB.Expressions;

using NuClear.AdvancedSearch.Replication.Model;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal static class MergeTool
    {
        private static readonly MethodInfo MergeMethodInfo = MemberHelper.MethodOf(() => Merge<IIdentifiable>(null, null)).GetGenericMethodDefinition();

        private static readonly ConcurrentDictionary<Type, MethodInfo> Methods = new ConcurrentDictionary<Type, MethodInfo>();

        public static IMergeResult Merge(IQueryable newData, IQueryable oldData)
        {
            if (newData.ElementType != oldData.ElementType)
            {
                throw new InvalidOperationException("The types are not matched.");
            }

            var type = newData.ElementType;
            var method = Methods.GetOrAdd(type, t => MergeMethodInfo.MakeGenericMethod(t));

            return (IMergeResult)method.Invoke(null, new object[] { newData, oldData });
        }

        public static MergeResult<T> Merge<T>(IEnumerable<T> data1, IEnumerable<T> data2)
        {
            using (var probe = new Probe("Merging " + typeof(T).Name))
            {
                var set1 = new HashSet<T>(data1);
                var set2 = new HashSet<T>(data2);

                // NOTE: avoiding enumerable extensions to reuse hashset performance
                var difference = set1.Where(x => !set2.Contains((T)x));
                var intersection = set1.Where(x => set2.Contains((T)x));
                var complement = set2.Where(x => !set1.Contains((T)x));

                return new MergeResult<T> { Difference = difference, Intersection = intersection, Complement = complement };
            }
        }

        public interface IMergeResult
        {
            IEnumerable Difference { get; }
            IEnumerable Intersection { get; }
            IEnumerable Complement { get; }
        }

        public class MergeResult<T> : IMergeResult
        {
            public IEnumerable<T> Difference { get; set; }
            public IEnumerable<T> Intersection { get; set; }
            public IEnumerable<T> Complement { get; set; }

            IEnumerable IMergeResult.Difference
            {
                get { return Difference; }
            }

            IEnumerable IMergeResult.Intersection
            {
                get { return Intersection; }
            }

            IEnumerable IMergeResult.Complement
            {
                get { return Complement; }
            }
        }
    }
}