using System.Diagnostics;

using NuClear.Model.Common;

namespace NuClear.Telemetry
{
    public sealed class DebugProfiler : IProfiler
    {
        public void Report<T>(long value)
            where T : PerformanceIdentityBase<T>, new()
        {
            Debug.WriteLine(value, IdentityBase<T>.Instance.Name);
        }
    }
}