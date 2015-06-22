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
                { PrimaryProcessingDelayIdentity.Instance, new GraphiteMetadataElement("Primary_ProcessingDelay", GraphiteMetadataElement.CounterType.Gauge) },
                { FinalProcessingDelayIdentity.Instance, new GraphiteMetadataElement("Final_ProcessingDelay", GraphiteMetadataElement.CounterType.Gauge) },
                { PrimaryProcessedOperationCountIdentity.Instance, new GraphiteMetadataElement("Primary_OperationCountIdentity", GraphiteMetadataElement.CounterType.Counter) },
                { FinalProcessedOperationCountIdentity.Instance, new GraphiteMetadataElement("Final_OperationCount", GraphiteMetadataElement.CounterType.Counter) },
            };

        public GraphiteMetadataElement Get<T>() where T : PerformanceIdentityBase<T>, new()
        {
            GraphiteMetadataElement result;
            data.TryGetValue(PerformanceIdentityBase<T>.Instance, out result);
            return result;
        }
    }
}