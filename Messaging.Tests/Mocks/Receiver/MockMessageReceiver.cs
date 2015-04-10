using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuClear.AdvancedSearch.Messaging.ServiceBus;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;

namespace NuClear.AdvancedSearch.Messaging.Tests.Mocks.Receiver
{
    public sealed class MockMessageReceiver : IMessageReceiver
    {
        private readonly ITrackedUseCaseParser _parser = new TrackedUseCaseParser();
        private readonly IReadOnlyList<byte[]> _messageBytes;
        private readonly Action<IEnumerable<IMessage>, IEnumerable<IMessage>> _assertAction;

        public MockMessageReceiver(IReadOnlyList<byte[]> messageBytes, Action<IEnumerable<IMessage>, IEnumerable<IMessage>> assertAction)
        {
            _messageBytes = messageBytes;
            _assertAction = assertAction;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            var useCases = _messageBytes.Select(x =>
            {
                var stream = new MemoryStream(x);
                var useCase = _parser.Parse(stream);
                return useCase;
            });

            return useCases.ToList();
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            _assertAction(successfullyProcessedMessages, failedProcessedMessages);
        }
    }
}