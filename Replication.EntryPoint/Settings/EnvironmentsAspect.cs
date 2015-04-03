using NuClear.Settings;
using NuClear.Settings.API;

namespace Replication.EntryPoint.Settings
{
    public sealed class EnvironmentsAspect : ISettingsAspect, IEnvironmentSettings
    {
        private readonly StringSetting _targetEnvironmentName = ConfigFileSetting.String.Required("TargetEnvironmentName");
        private readonly StringSetting _entryPointName = ConfigFileSetting.String.Required("EntryPointName");

        public string EntryPointName
        {
            get { return _entryPointName.Value; }
        }

        public string EnvironmentName
        {
            get { return _targetEnvironmentName.Value; }
        }
    }
}