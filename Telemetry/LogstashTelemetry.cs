using System;
using System.Net.Sockets;
using System.Text;

using Newtonsoft.Json;

using NuClear.AdvancedSearch.Settings;
using NuClear.Telemetry.Logstash;

namespace NuClear.Telemetry
{
    public sealed class LogstashTelemetry : ITelemetry
    {
        private readonly IClientWrapper _client;
        private readonly IEnvironmentSettings _environmentSettings;

        public LogstashTelemetry(IEnvironmentSettings environmentSettings, ILogstashSettings logstashSettings)
        {
            _environmentSettings = environmentSettings;

            var scheme = logstashSettings.LogstashUri.Scheme;
            var host = logstashSettings.LogstashUri.Host;
            var port = logstashSettings.LogstashUri.Port;

            switch (scheme)
            {
                case "udp":
                    _client = new UdpClientWrapper(host, port);
                    break;
                case "tcp":
                    _client = new TcpClientWrapper(host, port);
                    break;
                default:
                    throw new ArgumentException(string.Format("Protocol '{0}' is not supported for logstash connection", scheme));
            }
            
        }

        public void Report<T>(long value)
            where T : PerformanceIdentityBase<T>, new()
        {
            var report = new
            {
                EntryPoint = _environmentSettings.EntryPointName,
                Environment = _environmentSettings.EnvironmentName,
                Name = PerformanceIdentityBase<T>.Instance.Name,
                Value = value,
            };

            _client.Send(Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(report)));
        }

        private interface IClientWrapper
        {
            void Send(byte[] data);
        }

        private class TcpClientWrapper : IClientWrapper
        {
            private static readonly byte[] NewLine = { 10 };
            private readonly string _host;
            private readonly int _port;
            private readonly TcpClient _client;
            private readonly object _sync;

            public TcpClientWrapper(string host, int port)
            {
                _host = host;
                _port = port;
                _sync = new object();
                _client = new TcpClient();
            }

            public void Send(byte[] data)
            {
                try
                {
                    lock (_sync)
                    {
                        if (!_client.Connected)
                        {
                            _client.Connect(_host, _port);
                        }

                        var s = _client.GetStream();
                        s.Write(data, 0, data.Length);
                        s.Write(NewLine, 0, NewLine.Length);
                        s.Flush();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private class UdpClientWrapper : IClientWrapper
        {
            private readonly UdpClient _client;

            public UdpClientWrapper(string host, int port)
            {
                _client = new UdpClient(host, port);
            }

            public void Send(byte[] data)
            {
                _client.Send(data, data.Length);
            }
        }
    }
}