using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.DI.Factories.Unity.Transformers.Resolvers;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Final;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Transformer
{
    public sealed class FinalMessageTransformerResolveStrategy : IMessageTransformerResolveStrategy
    {
        private readonly ISet<IMetadataElementIdentity> _appropriateProcessingFlowsRegistrar;

        public FinalMessageTransformerResolveStrategy(IMetadataProvider metadataProvider)
        {
            _appropriateProcessingFlowsRegistrar =
                new HashSet<IMetadataElementIdentity>(metadataProvider.GetConcreteMetadataOfKind<MetadataMessageFlowsIdentity, MessageFlowMetadata>()
                                                          .Where(PerformedOperations.IsPerformedOperationsFinalSource)
                                                          .Select(metadata => metadata.Identity));
        }

        public bool TryGetAppropriateTransformer(MessageFlowMetadata messageFlowMetadata, out Type resolvedFlowReceiverType)
        {
            if (!_appropriateProcessingFlowsRegistrar.Contains(messageFlowMetadata.Identity))
            {
                resolvedFlowReceiverType = null;
                return false;
            }

            resolvedFlowReceiverType = typeof(AggregateOperationDeserializationMessageTransformer);
            return true;
        }
    }
}
