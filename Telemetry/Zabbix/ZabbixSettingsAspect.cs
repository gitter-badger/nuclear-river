using System;

using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.Telemetry.Zabbix
{
    public sealed class ZabbixSettingsAspect : ISettingsAspect, IZabbixSettings
    {
        private readonly StringSetting _zabbixUri = ConfigFileSetting.String.Optional("ZabbixUri", string.Empty);

        Uri IZabbixSettings.ZabbixUri
        {
            get { return new Uri(_zabbixUri.Value); }
        }
    }
}