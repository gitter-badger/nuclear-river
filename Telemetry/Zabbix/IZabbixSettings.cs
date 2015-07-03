using System;

using NuClear.Settings.API;

namespace NuClear.Telemetry.Zabbix
{
    public interface IZabbixSettings : ISettings
    {
        Uri ZabbixUri { get; }
    }
}