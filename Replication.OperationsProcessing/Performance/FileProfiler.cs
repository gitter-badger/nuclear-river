using System.IO;

using NuClear.AdvancedSearch.Settings;
using NuClear.Model.Common;

namespace NuClear.Replication.OperationsProcessing.Performance
{
    public sealed class FileProfiler : IProfiler
    {
        private readonly IEnvironmentSettings _environmentSettings;

        public FileProfiler(IEnvironmentSettings environmentSettings)
        {
            _environmentSettings = environmentSettings;
        }

        public void Report<T>(long value)
            where T : PerformanceIdentityBase<T>, new()
        {
            var name = string.Format("{0}.{1}.txt", _environmentSettings.EntryPointName, _environmentSettings.EnvironmentName);
            var output = string.Format("{0}\t{1}\n", IdentityBase<T>.Instance.Name, value);
            File.AppendAllText(Path.Combine(@"d:\counters", name), output);
        }
    }
}