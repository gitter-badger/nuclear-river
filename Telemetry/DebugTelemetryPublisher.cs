using System.Diagnostics;

using NuClear.Model.Common;

namespace NuClear.Telemetry
{
    public sealed class DebugTelemetryPublisher : ITelemetryPublisher
    {
        public void Report<T>(long value)
            where T : TelemetryIdentityBase<T>, new()
        {
            Debug.WriteLine(value, IdentityBase<T>.Instance.Name);
        }
    }
}