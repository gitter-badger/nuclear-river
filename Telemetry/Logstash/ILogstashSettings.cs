using System;

using NuClear.Settings.API;

namespace NuClear.Telemetry.Logstash
{
    public interface ILogstashSettings : ISettings
    {
        Uri LogstashUri { get; }
    }
}
