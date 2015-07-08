using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using NuClear.AdvancedSearch.Settings;
using NuClear.Telemetry.Logstash;

namespace NuClear.Telemetry
{
    public sealed class LogstashTelemetryPublisher : ITelemetryPublisher, IDisposable
    {
        private readonly IClientWrapper _client;
        private readonly IEnvironmentSettings _environmentSettings;
        private bool _disposed;

        public LogstashTelemetryPublisher(IEnvironmentSettings environmentSettings, ILogstashSettings logstashSettings)
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

        ~LogstashTelemetryPublisher()
        {
            Dispose(false);
        }

        public void Trace(string message, object data, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
        {
            var report = new
                         {
                             EntryPoint = _environmentSettings.EntryPointName,
                             Environment = _environmentSettings.EnvironmentName,
                             Name = "Tracing",
                             Message = message,
                             Data = data,
                             MemberName = memberName,
                             SourceFilePath = sourceFilePath,
                             SourceLineNumber = sourceLineNumber,
                             Thread = Thread.CurrentThread.ManagedThreadId
                         };

            try
            {
                _client.SendAsync(Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(report)));
            }
            catch (Exception)
            {
            }
        }

        public void Publish<T>(long value)
            where T : TelemetryIdentityBase<T>, new()
        {
            var report = new
                         {
                             EntryPoint = _environmentSettings.EntryPointName,
                             Environment = _environmentSettings.EnvironmentName,
                             Indicator = new Dictionary<string, long> { { TelemetryIdentityBase<T>.Instance.Name, value } }
                         };

            try
            {
                _client.SendAsync(Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(report)));
            }
            catch(Exception)
            {
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                _client.Dispose();
            }
        }

        private interface IClientWrapper : IDisposable
        {
            Task SendAsync(byte[] data);
        }

        private class TcpClientWrapper : IClientWrapper
        {
            private static readonly byte[] NewLine = { 10 };
            private readonly string _host;
            private readonly int _port;
            private readonly object _sync = new object();
            private TcpClient _client;
            private bool _disposed;

            public TcpClientWrapper(string host, int port)
            {
                _host = host;
                _port = port;
            }

            ~TcpClientWrapper()
            {
                Dispose(false);
            }

            public Task SendAsync(byte[] data)
            {
                return Task.Factory.StartNew(() => Send(data));
            }

            private void Send(byte[] data)
            {
                lock (_sync)
                {
                    var client = GetClient();
                    try
                    {
                        var s = client.GetStream();
                        s.Write(data, 0, data.Length);
                        s.Write(NewLine, 0, NewLine.Length);
                        s.Flush();
                    }
                    catch (Exception)
                    {
                        client.Close();
                        throw;
                    }
                }
            }

            void IDisposable.Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            
            private TcpClient GetClient()
            {
                if (_client != null && _client.Connected)
                {
                    return _client;
                }

                return _client = new TcpClient(_host, _port);
            }

            private void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                if (disposing)
                {
                    if (_client != null)
                    {
                        _client.Close();
                    }
                }
            }
        }

        private class UdpClientWrapper : IClientWrapper
        {
            private readonly UdpClient _client;
            private bool _disposed;

            public UdpClientWrapper(string host, int port)
            {
                _client = new UdpClient(host, port);
            }

            ~UdpClientWrapper()
            {
                Dispose(false);
            }

            public Task SendAsync(byte[] data)
            {
                return _client.SendAsync(data, data.Length);
            }

            void IDisposable.Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                if (disposing)
                {
                    _client.Close();
                }
            }
        }
    }
}