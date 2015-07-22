using System.Linq;
using System.Xml.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Transports;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public sealed class StatisticsOperationAccumulator<TMessageFlow> : MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, StatisticsAggregatableMessage>
        where TMessageFlow : class, IMessageFlow, new()
    {
        protected override StatisticsAggregatableMessage Process(PerformedOperationsFinalProcessingMessage message)
        {
            var operations = message.FinalProcessings.Select(x => XElement.Parse(x.Context).DeserializeStatisticsOperation()).ToList();
            return new StatisticsAggregatableMessage
                   {
                       TargetFlow = MessageFlow,
                       Operations = operations,
                   };
        }
    }
}