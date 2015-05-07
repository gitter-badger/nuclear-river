using System.Linq;
using System.Xml.Linq;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Transports;

namespace NuClear.Replication.OperationsProcessing.Final
{
    // TODO {a.rechkalov, 24.04.2015}: Не нужна в принципе, но нужна для корректной работы
    public sealed class EmptyStrategy : MessageProcessingStrategyBase<Replicate2CustomerIntelligenceFlow, IMessage, AggregateOperationAggregatableMessage>
    {
        protected override AggregateOperationAggregatableMessage Process(IMessage message)
        {
            var actualMessage = (PerformedOperationsFinalProcessingMessage)message;
            var operations = actualMessage.FinalProcessings.Select(x => AggregateOperationSerialization.FromXml(XElement.Parse(x.Context)));

            return new AggregateOperationAggregatableMessage
                   {
                       TargetFlow = FakeFlow.Instance,
                       Operations = operations
                   };
        }
    }
}
