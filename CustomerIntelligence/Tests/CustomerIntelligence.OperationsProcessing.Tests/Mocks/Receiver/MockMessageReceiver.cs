using System;
using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Tests.Mocks.Receiver
{
    public sealed class MockMessageReceiver : IMessageReceiver
    {
        private readonly IReadOnlyList<IMessage> _messages;
        private readonly Action<IEnumerable<IMessage>, IEnumerable<IMessage>> _assertAction;

        public MockMessageReceiver(IReadOnlyList<IMessage> messages, Action<IEnumerable<IMessage>, IEnumerable<IMessage>> assertAction)
        {
            _messages = messages;
            _assertAction = assertAction;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            return _messages;
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            _assertAction(successfullyProcessedMessages, failedProcessedMessages);
        }
    }
}