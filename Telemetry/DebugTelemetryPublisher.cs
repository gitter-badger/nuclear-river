using System.Diagnostics;

using NuClear.Model.Common;

namespace NuClear.Telemetry
{
    public sealed class DebugTelemetryPublisher : ITelemetryPublisher
    {
        public void Publish<T>(long value)
            where T : TelemetryIdentityBase<T>, new()
        {
            Debug.WriteLine(value, IdentityBase<T>.Instance.Name);
        }

        public void Trace(string message, object data)
        {
            Debug.WriteLine(message);
            Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }
    }
}