using System.Runtime.CompilerServices;

namespace NuClear.Telemetry
{
    public interface ITelemetryPublisher
    {
        void Publish<T>(long value)
            where T : TelemetryIdentityBase<T>, new();

        void Trace(string message,
                   object data = null,
                   [CallerMemberName] string memberName = "",
                   [CallerFilePath] string sourceFilePath = "",
                   [CallerLineNumber] int sourceLineNumber = 0);
    }
}