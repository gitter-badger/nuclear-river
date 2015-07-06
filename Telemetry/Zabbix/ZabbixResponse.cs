using System;

using Newtonsoft.Json;

namespace NuClear.Telemetry.Zabbix
{
    public sealed class ZabbixResponse
    {
        [JsonProperty(PropertyName = "response")]
        public string Response { get; set; }

        [JsonProperty(PropertyName = "info")]
        public string Info { get; set; }

        public bool Success { get { return string.Equals(Response, "success", StringComparison.OrdinalIgnoreCase); } }
    }
}