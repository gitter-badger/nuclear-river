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

                return new MergeResult<T>(difference, intersection, complement);
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class MergeResult<T> : IMergeResult, IMergeResult<T>
    {
        public IEnumerable<T> Difference { get; private set; }
        public IEnumerable<T> Intersection { get; private set; }
        public IEnumerable<T> Complement { get; private set; }

        public MergeResult(IEnumerable<T> difference, IEnumerable<T> intersection, IEnumerable<T> complement)
        {
            Difference = difference;
            Intersection = intersection;
            Complement = complement;
        }

        public MergeResult(IMergeResult<T> mergeResult)
        {
            Difference = mergeResult.Difference;
            Intersection = mergeResult.Intersection;
            Complement = mergeResult.Complement;
        }

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