using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Stages;

namespace NuClear.Replication.OperationsProcessing.Metadata.Flows
{
    public sealed class PerformedOperationsMessageFlowsMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
    {
        private static readonly HierarchyMetadata MetadataRoot =
            PerformedOperations.Flows
                               .Primary(MessageFlowMetadata.Config.For<Replicate2CustomerIntelligenceFlow>()
                                                           .Strategy<FilteringStrategy>()
                                                           .Handler<FactProcessingHandler>()
                                                           .To.Primary().Flow<Replicate2CustomerIntelligenceFlow>().Connect
                                                           .To.Final().Flow<Replicate2CustomerIntelligenceFlow>().Connect)
                               .Final(MessageFlowMetadata.Config.For<Replicate2CustomerIntelligenceFlow>()
                                                         .Strategy<EmptyStrategy>()
                                                         .To.Final().Flow<Replicate2CustomerIntelligenceFlow>().Connect);
        
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
