using System.Configuration;

using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Web.OData.Settings
{
    internal sealed class TracerSettingsAspect : ISettingsAspect, ITracerSettings
    {
        private readonly StringSetting _targetEnvironmentName = ConfigFileSetting.String.Required("TargetEnvironmentName");
        private readonly StringSetting _entryPointName = ConfigFileSetting.String.Required("EntryPointName");
        private readonly string _connectionString;

        public TracerSettingsAspect()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Logging"].ConnectionString;
        }

        public string EntryPointName
        {
            get { return _entryPointName.Value; }
        }

        public string EnvironmentName
        {
            get { return _targetEnvironmentName.Value; }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }
    }
}