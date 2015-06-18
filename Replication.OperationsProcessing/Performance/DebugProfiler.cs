using System.Diagnostics;

using NuClear.AdvancedSearch.Settings;
using NuClear.Model.Common;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class DebugProfiler : IProfiler
    {
        private readonly IEnvironmentSettings _environmentSettings;

        public DebugProfiler(IEnvironmentSettings environmentSettings)
        {
            _environmentSettings = environmentSettings;
        }

        public void Report<T>(long value)
            where T : PerformanceIdentityBase<T>, new()
        {
            Debug.WriteLine(value, IdentityBase<T>.Instance.Name);
        }
    }
}