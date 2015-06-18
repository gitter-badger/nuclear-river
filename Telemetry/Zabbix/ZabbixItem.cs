using System;

using Newtonsoft.Json;

namespace NuClear.Telemetry.Zabbix
{
    public sealed class ZabbixItem
    {
        [JsonProperty(PropertyName = "host")]
        public string Host { get; set; }

        [JsonProperty(PropertyName = "key")]
        public string ItemKey { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "clock")]
        public long UnixTimeStamp = ZabbixSender.UnixTimeConverter.ToUnixTimestamp(DateTime.UtcNow);
    }
}