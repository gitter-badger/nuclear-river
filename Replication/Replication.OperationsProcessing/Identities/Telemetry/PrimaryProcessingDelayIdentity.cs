using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Identities.Telemetry
{
    public sealed class PrimaryProcessingDelayIdentity : TelemetryIdentityBase<PrimaryProcessingDelayIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "�������� ������� ����� ����������� TUC � ERM � ��� ���������� � AS, ��"; }
        }
    }
}