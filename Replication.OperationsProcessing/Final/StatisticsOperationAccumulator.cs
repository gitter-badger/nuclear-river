using System.Linq;
using System.Xml.Linq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Transports;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public sealed class StatisticsOperationAccumulator<TMessageFlow> : MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, OperationAggregatableMessage<CalculateStatisticsOperation>>
        where TMessageFlow : class, IMessageFlow, new()
    {
        protected override OperationAggregatableMessage<CalculateStatisticsOperation> Process(PerformedOperationsFinalProcessingMessage message)
        {
            var operations = message.FinalProcessings.Select(x => XElement.Parse(x.Context).DeserializeStatisticsOperation()).ToList();
            var oldestOperation = message.FinalProcessings.Min(x => x.CreatedOn);

            return new OperationAggregatableMessage<CalculateStatisticsOperation>
            {
                TargetFlow = MessageFlow,
                Operations = operations,
                OperationTime = oldestOperation,
            };
        }
    }
}