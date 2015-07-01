using System;

using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.Telemetry.Logstash
{
    public sealed class LogstashSettingsAspect : ISettingsAspect, ILogstashSettings
    {
        private readonly StringSetting _uri = ConfigFileSetting.String.Required("LogstashUri");

        Uri ILogstashSettings.LogstashUri
        {
            get { return new Uri(_uri.Value); }
        }
    }
}