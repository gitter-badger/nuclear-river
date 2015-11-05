using System;
using System.Collections.Generic;

namespace NuClear.DataTest.Runner.Comparer
{
    public class CompareResult
    {
        public CompareResult(IReadOnlyCollection<object> unexpected, IReadOnlyCollection<object> missing, IReadOnlyCollection<Tuple<object, object>> wrong)
        {
            Missing = missing;
            Unexpected = unexpected;
            Wrong = wrong;
        }

        public IReadOnlyCollection<object> Missing { get; }
        public IReadOnlyCollection<object> Unexpected { get; }
        public IReadOnlyCollection<Tuple<object, object>> Wrong { get; }
    }
}