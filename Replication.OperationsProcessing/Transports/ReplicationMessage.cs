using System;
using System.Collections.Generic;

using NuClear.Messaging.API;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public class ReplicationMessage<TElement> : IMessage
    {
        public Guid Id { get; set; }
        public IReadOnlyCollection<TElement> Operations { get; set; }

        public bool Equals(IMessage other)
        {
            return Equals(this.Id, other.Id);
        }
    }
}