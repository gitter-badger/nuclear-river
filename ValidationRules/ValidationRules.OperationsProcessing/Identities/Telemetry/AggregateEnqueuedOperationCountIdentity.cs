using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Telemetry
{
    public sealed class ConfigUpdateCountIdentity : TelemetryIdentityBase<ConfigUpdateCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количиство принятых обновления конфига сопутствия-запрещения"; }
        }
    }
}