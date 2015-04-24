using System;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;

namespace NuClear.Replication.OperationsProcessing.Stages
{
    // TODO {a.rechkalov, 23.04.2015}: Контейнер или одноимённый тип из feature/ERM-6013
    public class FactOperation : IAggregatableMessage
    {
        public bool Equals(IMessage other)
        {
            return ReferenceEquals(this, other);
        }

        public Guid Id { get; set; }
        public IMessageFlow TargetFlow { get; set; }
    }

    // TODO {a.rechkalov, 23.04.2015}: Контейнер или одноимённый тип из feature/ERM-6013
    public class AggregateOperation : IAggregatableMessage
    {
        public bool Equals(IMessage other)
        {
            return ReferenceEquals(this, other);
        }

        public Guid Id { get; set; }
        public IMessageFlow TargetFlow { get; set; }
    }
}