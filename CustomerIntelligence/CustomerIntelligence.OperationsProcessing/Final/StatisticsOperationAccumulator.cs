using System.Linq;
using System.Xml.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class StatisticsOperationAccumulator<TMessageFlow> : MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, OperationAggregatableMessage<RecalculateStatisticsOperation>>
        where TMessageFlow : class, IMessageFlow, new()
    {
        protected override OperationAggregatableMessage<RecalculateStatisticsOperation> Process(PerformedOperationsFinalProcessingMessage message)
        {
            var operations = message.FinalProcessings.Select(x => XElement.Parse(x.Context).DeserializeStatisticsOperation()).ToArray();
            var oldestOperation = message.FinalProcessings.Min(x => x.CreatedOn);

            return new OperationAggregatableMessage<RecalculateStatisticsOperation>
            {
                TargetFlow = MessageFlow,
                Operations = operations,
                OperationTime = oldestOperation,
            };
        }
    }
}