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

        public void Trace(string message, object data = null, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
        {
            throw new System.NotImplementedException();
        }
    }
}