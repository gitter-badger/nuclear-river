using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface IMergeResult
    {
        IEnumerable Difference { get; }
        IEnumerable Intersection { get; }
        IEnumerable Complement { get; }
    }

    public interface IMergeResult<out T>
    {
        IEnumerable<T> Difference { get; }
        IEnumerable<T> Intersection { get; }
        IEnumerable<T> Complement { get; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class MergeTool
    {
        public static MergeResult<T> Merge<T>(IEnumerable<T> data1, IEnumerable<T> data2)
        {
            using (Probe.Create("Merging", typeof(T).Name))
            {
                var set1 = new HashSet<T>(data1);
                var set2 = new HashSet<T>(data2);

                // NOTE: avoiding enumerable extensions to reuse hashset performance
                var difference = set1.Where(x => !set2.Contains(x));
                var intersection = set1.Where(x => set2.Contains(x));
                var complement = set2.Where(x => !set1.Contains(x));

                return new MergeResult<T> { Difference = difference, Intersection = intersection, Complement = complement };
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class MergeResult<T> : IMergeResult, IMergeResult<T>
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