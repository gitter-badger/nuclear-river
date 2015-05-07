using System.Linq;
using System.Xml.Linq;

using NuClear.Messaging.API.Processing.Actors.Transformers;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Transports;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public sealed class AggregateOperationDeserializationMessageTransformer : MessageTransformerBase<PerformedOperationsFinalProcessingMessage, AggregateOperationAggregatableMessage>
    {
        protected override AggregateOperationAggregatableMessage Transform(PerformedOperationsFinalProcessingMessage message)
        {
            var operations = message.FinalProcessings.Select(x => AggregateOperationSerialization.FromXml(XElement.Parse(x.Context)));

            return new AggregateOperationAggregatableMessage
            {
                TargetFlow = FakeFlow.Instance,
                Operations = operations
            };
        }
    }
}
