using System.Collections.Generic;

using NuClear.Model.Common;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Telemetry;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
{
    internal class GraphiteCounterMetadata : IGraphiteCounterMetadata
    {
        private static readonly IDictionary<IIdentity, GraphiteMetadataElement> data =
            new Dictionary<IIdentity, GraphiteMetadataElement>()
            {
                { PrimaryProcessingDelayIdentity.Instance, new GraphiteMetadataElement("PrimaryProcessingDelay", GraphiteMetadataElement.CounterType.Gauge) },
                { FinalProcessingDelayIdentity.Instance, new GraphiteMetadataElement("FinalProcessingDelay", GraphiteMetadataElement.CounterType.Gauge) },
                { ErmFactOperationProcessedCountIdentity.Instance, new GraphiteMetadataElement("FactOperation", GraphiteMetadataElement.CounterType.Counter) },
                { BitStatisticsEntityProcessedCountIdentity.Instance, new GraphiteMetadataElement("BitEntity", GraphiteMetadataElement.CounterType.Counter) },
                { AggregateOperationProcessedCountIdentity.Instance, new GraphiteMetadataElement("AggregateOperation", GraphiteMetadataElement.CounterType.Counter) },
                { ErmOperationCountIdentity.Instance, new GraphiteMetadataElement("ErmOperation", GraphiteMetadataElement.CounterType.Counter) },
            };

        public GraphiteMetadataElement Get<T>() where T : PerformanceIdentityBase<T>, new()
        {
            GraphiteMetadataElement result;
            data.TryGetValue(PerformanceIdentityBase<T>.Instance, out result);
            return result;
        }
    }
}