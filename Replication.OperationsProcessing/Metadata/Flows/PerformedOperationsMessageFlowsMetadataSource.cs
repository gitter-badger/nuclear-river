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
                               .Primary(

                                    MessageFlowMetadata.Config.For<ImportFactsFromErmFlow>()
                                    .Strategy<ImportFactsFromErmAccumulator>()
                                    .Handler<ImportFactsFromErmHandler>()
                                    .To.Primary().Flow<ImportFactsFromErmFlow>().Connect()
                                    .To.Final().Flow<ImportFactsFromErmFlow>().Connect(),

                                    MessageFlowMetadata.Config.For<ImportFactsFromBitFlow>()
                                    .Strategy<ImportFactsFromBitAccumulator>()
                                    .Handler<ImportFactsFromBitHandler>()
                                    .To.Primary().Flow<ImportFactsFromBitFlow>().Connect()
                                    .To.Final().Flow<ImportFactsFromBitFlow>().Connect()

                                    )
                               .Final(

                                    MessageFlowMetadata.Config.For<ImportFactsFromErmFlow>()
                                    .Strategy<AggregateOperationAccumulator<ImportFactsFromErmFlow>>()
                                    .Handler<AggregateOperationAggregatableMessageHandler>()
                                    .To.Final().Flow<ImportFactsFromErmFlow>().Connect(),

                                    MessageFlowMetadata.Config.For<ImportFactsFromBitFlow>()
                                    .Strategy<AggregateOperationAccumulator<ImportFactsFromBitFlow>>()
                                    .Handler<AggregateOperationAggregatableMessageHandler>()
                                    .To.Final().Flow<ImportFactsFromBitFlow>().Connect()

                                    );

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
