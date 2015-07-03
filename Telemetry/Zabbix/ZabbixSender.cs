using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace NuClear.Telemetry.Zabbix
{
    public sealed class ZabbixSender : IDisposable
    {
        private readonly TcpClient _tcpClient;
        private readonly JsonSerializerSettings _settings;

        public ZabbixSender(Uri zabbixUri)
        {
            var port = !zabbixUri.IsDefaultPort ? zabbixUri.Port : 10051;

            _tcpClient = new TcpClient(zabbixUri.Host, port);
            _settings = new JsonSerializerSettings { Formatting = Formatting.None };
        }

        public async Task<ZabbixResponse> Send(IEnumerable<ZabbixItem> items)
        {
            var request = new ZabbixRequest { Items = items };

            var networkStream = _tcpClient.GetStream();
            using (var reader = new StreamReader(networkStream))
            using (var writer = new StreamWriter(networkStream) { AutoFlush = true })
            {
                var writeData = JsonConvert.SerializeObject(request, _settings);

                await writer.WriteAsync(writeData);
                var readData = await reader.ReadLineAsync();

                readData = readData.Substring(readData.IndexOf('{'));
                return JsonConvert.DeserializeObject<ZabbixResponse>(readData);
            }
        }

        public void Dispose()
        {
            _tcpClient.Close();
        }

        private sealed class ZabbixRequest
        {
            [JsonProperty(PropertyName = "request")]
            public string Request = "sender data";

            [JsonProperty(PropertyName = "data")]
            public IEnumerable<ZabbixItem> Items { get; set; }

            [JsonProperty(PropertyName = "clock")]
            public long UnixTimeStamp = UnixTimeConverter.ToUnixTimestamp(DateTime.UtcNow);
        }

        public static class UnixTimeConverter
        {
            public static long ToUnixTimestamp(DateTime dateTime)
            {
                return dateTime.Ticks / 10000L - 62131301832704L;
            }
        }
    }
}