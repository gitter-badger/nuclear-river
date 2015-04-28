using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Final;
using NuClear.Replication.OperationsProcessing.Primary;

namespace NuClear.Replication.OperationsProcessing.Metadata.Flows
{
    public sealed class PerformedOperationsMessageFlowsMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
    {
        private static readonly HierarchyMetadata MetadataRoot =
            PerformedOperations.Flows
                               .Primary(MessageFlowMetadata.Config.For<Replicate2CustomerIntelligenceFlow>()
                                                           .To.Final().Flow<Replicate2CustomerIntelligenceFlow>().Connect)
                               .Final(MessageFlowMetadata.Config.For<Replicate2CustomerIntelligenceFlow>()
                                                         .Strategy<PerformedOperationsFilteringStrategy>()
                                                         .Handler<ErmToFactReplicationHandler>()
                                                         .To.Final().Flow<FakeFlow>().Connect)
                               .Final(MessageFlowMetadata.Config.For<FakeFlow>()
                                                         .Strategy<EmptyStrategy>()
                                                         .Handler<ReplicateToCustomerIntelligenceMessageAggregatedProcessingResultHandler>());
        
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public PerformedOperationsMessageFlowsMetadataSource()
        {
            _metadata = new Dictionary<Uri, IMetadataElement> { { MetadataRoot.Identity.Id, MetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}
