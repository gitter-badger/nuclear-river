using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.ServiceBus.Messaging;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.ServiceBus.API;

namespace NuClear.AdvancedSearch.Messaging.Tests.Mocks
{
    internal sealed class MockServiceBusMessageFlowReceiverFactory : IServiceBusMessageFlowReceiverFactory
    {
        private readonly IEnumerable<byte[]> _messageBytes;

        public MockServiceBusMessageFlowReceiverFactory(IEnumerable<byte[]> messageBytes)
        {
            _messageBytes = messageBytes;
        }

        public IServiceBusMessageFlowReceiver Create(IMessageFlow messageFlow)
        {
            return new MockServiceBusMessageFlowReceiver(this);
        }

        private sealed class MockServiceBusMessageFlowReceiver : IServiceBusMessageFlowReceiver
        {
            private readonly MockServiceBusMessageFlowReceiverFactory _parent;

            public MockServiceBusMessageFlowReceiver(MockServiceBusMessageFlowReceiverFactory parent)
            {
                _parent = parent;
            }

            public void Dispose()
            {
            }

            public IEnumerable<BrokeredMessage> ReceiveBatch(int messageCount)
            {
                return _parent._messageBytes.Select(x =>
                {
                    var message = new BrokeredMessage(new MemoryStream(x), true);
                    return message;
                });
            }

            public void CompleteBatch(IEnumerable<Guid> lockTokens)
            {
                throw new NotImplementedException();
            }
        }
    }
}
