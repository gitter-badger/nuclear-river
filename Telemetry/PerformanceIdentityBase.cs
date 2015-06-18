using NuClear.Model.Common;

namespace NuClear.Telemetry
{
    public abstract class PerformanceIdentityBase<T> : IdentityBase<T> 
        where T : IdentityBase<T>, new()
    {
        public abstract string Name { get; }
    }
}